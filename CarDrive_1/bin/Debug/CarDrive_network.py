import time

import tensorflow as tf
import random
from collections import deque
import numpy as np

import clr
clr.AddReference("CarDrive_1")
import CarDrive_1 as car

from Thread_Timer import Thread_Timer as TT
from threading import Timer


class DQN:
    def __init__(self, session, input_size):
        self.session = session
        self.input_size = input_size
        self.Variable_initializer = tf.contrib.layers.xavier_initializer()

        self._build_network()

    def _build_network(self):
        self.X = tf.placeholder(tf.float32, [None, self.input_size])
        self.X_gb = tf.placeholder(tf.float32, [None, self.input_size + 1])
        self.Y_gb = tf.placeholder(tf.float32, [None, 3])
        self.Y_lr = tf.placeholder(tf.float32, [None, 3])
        h_size = 128
        l_rate = 0.001

        w1 = self.build_weight("W1", [self.input_size, h_size], self.X)
        w2 = self.build_weights("w2", 3, h_size, w1)
        w3 = tf.get_variable("W3", [h_size, h_size], initializer=self.Variable_initializer)
        w4 = tf.matmul(w2, w3)
        gb = self.build_weight("GB", [h_size, h_size / 2], w4)
        gb2 = self.build_weight("GB2", [h_size / 2, h_size / 2], gb)

        w11 = self.build_weight("W11", [self.input_size + 1, h_size], self.X_gb)
        w22 = self.build_weights("w22", 3, h_size, w11)
        w33 = tf.get_variable("W33", [h_size, h_size], initializer=self.Variable_initializer)
        w44 = tf.matmul(w22, w33)
        lr = self.build_weight("LR", [h_size, h_size / 2], w44)
        lr2 = self.build_weight("LR2", [h_size / 2, h_size / 2], lr)

        gb_last = tf.get_variable("gb_l", [h_size / 2, 3], initializer=self.Variable_initializer)
        lr_last = tf.get_variable("lr_l", [h_size / 2, 3], initializer=self.Variable_initializer)

        self.Predict_gb = tf.matmul(gb2, gb_last)
        self.Predict_lr = tf.matmul(lr2, lr_last)

        self.loss_gb = tf.reduce_mean(tf.square(self.Y_gb - self.Predict_gb))
        self.loss_lr = tf.reduce_mean(tf.square(self.Y_lr - self.Predict_lr))

        self.train_gb = tf.train.AdamOptimizer(learning_rate=l_rate).minimize(self.loss_gb)
        self.train_lr = tf.train.AdamOptimizer(learning_rate=l_rate).minimize(self.loss_lr)
        #w4 = tf.get_variable("W4", [h_size, self.output_size], initializer=self.Variable_initializer)

        #self.Predict_Q = tf.matmul(w4, tokeyinput)

        #self.loss_function = tf.reduce_mean(tf.square(self.Y - self.Predict_Q))
        #self.train = tf.train.AdamOptimizer(learning_rate=l_rate).minimize(self.loss_function)

    def build_weight(self, name, shape, input):
        w = tf.get_variable(name, shape, initializer=self.Variable_initializer)
        layer = tf.nn.relu(tf.matmul(input, w))
        return layer

    def build_weights(self, name, no, size, input):
        front_w = tf.get_variable(name + "auto_w_1", [size, size], initializer=self.Variable_initializer)
        front_b = tf.get_variable(name + "auto_b_1", [size], initializer=self.Variable_initializer)
        layer = tf.nn.relu(tf.matmul(input, front_w) + front_b)
        for i in range(no):
            w = tf.get_variable(name + "auto_w" + str(i), [size, size], initializer=self.Variable_initializer)
            b = tf.get_variable(name + "auto_b" + str(i), [size], initializer=self.Variable_initializer)
            new_layer = tf.nn.relu(tf.matmul(layer, w) + b)
            layer = new_layer

        return layer

    def predict_gb(self, m, lr = -1):
        #mm = np.hstack((m, lr))
        return self.session.run(self.Predict_gb, feed_dict={self.X: m})

    def predict_lr(self, m):
        return self.session.run(self.Predict_lr, feed_dict={self.X: m})

    def update_gb(self, x_stack, y_stack):
        return self.session.run([self.loss_gb, self.train_gb],
                                feed_dict={self.X: x_stack, self.Y_gb: y_stack})

    def update_lr(self, x_stack, y_stack):
        return self.session.run([self.loss_lr, self.train_lr],
                                feed_dict={self.X: x_stack, self.Y_lr: y_stack})


class Module:
    def __init__(self, numofcar, in_size):
        self.stepterm = 0.01  #10ms

        self.numofcar = numofcar
        self.input_size = in_size
        self.output_size = 9

        self.session = tf.Session()
        self.dis = 0.75
        self.R_Memory = 500000
        self.play_count = 0
        self.step_count = 0

        #self.trainer = None
        self.dqn = None
        self.CarDrive = None
        self.replay_buffer = deque()

        self.state = []
        self.reward = []
        self.Done = []

        self.env_setting()

    def env_setting(self):

        #for i in range(self.numofcar):
            #self.alivecar.append(True)

        self.dqn = DQN(self.session, self.input_size)
        tf.global_variables_initializer().run(session=self.session)
        #카 드라이브 객체 생성 후 차 갯수 설정
        self.CarDrive = CConnecter(self.numofcar)
        return True

    #dqn을 훈련시키는 코드
    def replay_train(self):
        replaytime = 4
        batch_size = 1000
        if batch_size > len(self.replay_buffer):
            batch_size = len(self.replay_buffer)

        for _ in range(replaytime):
            trainbatch = random.sample(self.replay_buffer, batch_size)

            x_stack = np.empty(0).reshape(0, self.input_size)
            y_stack_gb = np.empty(0).reshape(0, 3)
            y_stack_lr = np.empty(0).reshape(0, 3)
            #print(trainbatch[0])

            for state, action, reward, next_state, done in trainbatch:
                Q_lr = self.dqn.predict_lr(state)
                Q_gb = self.dqn.predict_gb(state)
                predict_lr = np.max(self.dqn.predict_lr(next_state), 1)
                predict_gb = np.max(self.dqn.predict_gb(next_state), 1)

                for i in range(len(Q_gb)):
                    a = action[i]
                    if done[i]:
                        Q_gb[i, a // 3] = reward[i]
                        Q_lr[i, a % 3] = reward[i]
                    else:
                        Q_gb[i, a // 3] = (1 - self.dis) * reward[i] + self.dis * predict_gb[i]
                        Q_lr[i, a % 3] = (1 - self.dis) * reward[i] + self.dis * predict_lr[i]

                x_stack = np.vstack([x_stack, state])
                y_stack_gb = np.vstack([y_stack_gb, Q_gb])
                y_stack_lr = np.vstack([y_stack_lr, Q_lr])
            loss, _ = self.dqn.update_gb(x_stack, y_stack_gb)
            loss_l, _ = self.dqn.update_lr(x_stack, y_stack_lr)
            print("Loss : ", loss, "\n\t", loss_l)

        print("Update\n")

    #훈련
    def trainstart(self):
        #self.CarDrive.Reset(self.play_count)

        a = []
        for _ in range(self.numofcar):
            a.append(4)
        self.state, self.reward, self.Done = self.step(a)

        while self.CarDrive.Run:
            self.select_action()
            time.sleep(self.stepterm)

    #실제 실행되는 코드 - 액션을 선택하고 결과를 리턴받음
    def select_action(self):
        #숫자 하나 내보내고, 숫자[3 / 3], reward, Done 배열 받음


        e = 0.9 / ((self.play_count / 3) + 1)
        randid = np.random.rand(len(self.state))

        if (self.play_count + 1) * 3 < self.output_size:  #초반 앞으로좀 가게 하기 위한 장치
            bound = (self.play_count + 1) * 3
        else:
            bound = self.output_size


        predict_lr = self.dqn.predict_lr(self.state)
        #predict = np.argmax(predict_gb, 1) * 3 + np.argmax(predict_lr, 1)
        lraction = []
        for i in range(len(self.state)):
            if randid[i] < e:
                a = np.random.rand(1) * bound
                lraction.append(int(a[0] % self.output_size))
            else:
                lraction.append(np.argmax(predict_lr[i]))

        predict_gb = self.dqn.predict_gb(self.state, lraction)
        gbaction = []
        for i in range(len(self.state)):
            if randid[i] < e:
                a = np.random.rand(1) * bound
                gbaction.append(int(a[0] % self.output_size))
            else:
                gbaction.append(np.argmax(predict_gb[i]))
        action = gbaction * 3 + lraction
        next_state, self.reward, self.Done = self.step(action)

        self.replay_buffer.append((self.state, action, self.reward, next_state, self.Done))

        if(next_state == None): return
        n_state = next_state[:]
        n = 0
        for i in range(len(n_state)):
            if self.Done[i]:
                n_state.pop(i - n)
                n += 1

        self.state = n_state

        if len(self.replay_buffer) > self.R_Memory:
            self.replay_buffer.popleft()

        self.step_count += 1

        if self.step_count % (600 + self.play_count * 0) == 0:
            self.replay_train()

        #if self.step_count > 9999999:
           # self.Reset()

        if not n_state:
            self.Reset()

        #if self.play_count > 1000:
        #    self.CarDrive.Stop()

    #C#과 통신
    def step(self, next_move):
        state, reward, Done = self.CarDrive.Move(next_move)
        return state, reward, Done

    def Reset(self):
        self.step_count = 0
        self.play_count += 1
        self.replay_train()
        print(self.play_count, " Done\n")
        time.sleep(0.3)
        if not self.CarDrive.Reset(self.play_count):
            self.CarDrive.Stop()

        a = []
        for _ in range(self.numofcar):
            a.append(4)
        self.state, self.reward, self.Done = self.step(a)

class CConnecter:
    def __init__(self, num):
        self.form = car.Program.ExMain()
        self.Cmodule = self.form.getMainProgram()
        self.Run = True
        self.Set(num)


    def Set(self, carno):
        self.Cmodule.AddCar(carno)

    def Move(self, onehot):
        if self.Cmodule == None:
            self.Stop()
            return None, None, None
        if not self.Cmodule.check():
            self.Stop()
            return None, None, None

        ans = self.Cmodule.Request_Move(onehot)


        state = []
        reward = []
        Done = []

        for car_no in range(len(ans)):
            c_state = ans[car_no].get_Item1()
            state.append([])
            for j in range(len(c_state)):
                state[car_no].append(c_state[j])

            c_reward = ans[car_no].get_Item2()
            reward.append(0 + c_reward)
            Done.append(ans[car_no].get_Item3())

        return state, reward, Done

    def Reset(self, count):
        self.Cmodule.playcount(count)
        return self.Cmodule.Reset()

    def Stop(self):
        self.Run = False



#form = CarDrive_1.Program.ExMain()
module = Module(5, 6) #carnum = 5, inputnum = 6
module.trainstart()


