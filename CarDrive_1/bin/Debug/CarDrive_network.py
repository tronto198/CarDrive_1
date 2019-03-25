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
    def __init__(self, session, input_size, output_size):
        self.session = session
        self.input_size = input_size
        self.output_size = output_size
        self.Variable_initializer = tf.contrib.layers.xavier_initializer()

        self._build_network()

    def _build_network(self):
        self.X = tf.placeholder(tf.float32, [None, self.input_size])
        self.Y = tf.placeholder(tf.float32, [None, self.output_size])
        h_size = 1024
        l_rate = 0.001

        w1 = self.build_weight("W1", [self.input_size, h_size], self.X)
        w3 = self.build_weights(3, h_size, w1)
        w4 = tf.get_variable("W4", [h_size, self.output_size], initializer=self.Variable_initializer)

        self.Predict_Q = tf.matmul(w3, w4)

        self.loss_function = tf.reduce_mean(tf.square(self.Y - self.Predict_Q))
        self.train = tf.train.AdamOptimizer(learning_rate=l_rate).minimize(self.loss_function)

    def build_weight(self, name, shape, input):
        w = tf.get_variable(name, shape, initializer=self.Variable_initializer)
        layer = tf.nn.relu(tf.matmul(input, w))
        return layer

    def build_weights(self, no, size, input):
        front_w = tf.get_variable("auto_w_1", [size, size], initializer=self.Variable_initializer)
        layer = tf.nn.relu(tf.matmul(input, front_w))
        for i in range(no):
            w = tf.get_variable("auto_w" + str(i), [size, size], initializer=self.Variable_initializer)
            new_layer = tf.nn.relu(tf.matmul(layer, w))
            layer = new_layer

        return layer

    def predict(self, m):
        return self.session.run(self.Predict_Q, feed_dict={self.X: m})

    def update(self, x_stack, y_stack):
        return self.session.run([self.loss_function, self.train],
                                feed_dict={self.X: x_stack, self.Y: y_stack})


class Module:
    def __init__(self, numofcar, in_size, out_size):
        self.numofcar = numofcar
        self.input_size = in_size
        self.output_size = out_size

        self.session = tf.Session()
        self.dis = 0.6
        self.R_Memory = 1000000
        self.play_count = 0
        self.step_count = 0

        #self.trainer = None
        self.dqn = None
        self.CarDrive = CConnecter(numofcar)
        self.replay_buffer = deque()

        self.state = []
        self.reward = 0
        self.Done = False

        self.env_setting()

    def env_setting(self):

        #for i in range(self.numofcar):
            #self.alivecar.append(True)

        self.dqn = DQN(self.session, self.input_size, self.output_size)
        tf.global_variables_initializer().run(session=self.session)
        #카 드라이브 객체 생성 후 차 갯수 설정
        return True

    #dqn을 훈련시키는 코드
    def replay_train(self):
        replaytime = 5
        batch_size = 150
        for _ in range(replaytime):
            if batch_size > len(self.replay_buffer):
                batch_size = len(self.replay_buffer)
            trainbatch = random.sample(self.replay_buffer, batch_size)

            x_stack = np.empty(0).reshape(0, self.input_size)
            y_stack = np.empty(0).reshape(0, self.output_size)
            #print(trainbatch[0])

            for state, action, reward, next_state, done in trainbatch:
                Q = self.dqn.predict(state)

                if done:
                    Q[0, action] = reward
                else:
                    Q[0, action] = (1 - self.dis) * reward + self.dis * np.max(self.dqn.predict(next_state))

                x_stack = np.vstack([x_stack, state])
                y_stack = np.vstack([y_stack, Q])
            loss, _ = self.dqn.update(x_stack, y_stack)
            print("Loss : ", loss)

        print("Update\n")

    #훈련
    def trainstart(self):
        self.CarDrive.Reset(self.play_count)
        self.state, self.reward, self.Done = self.step(4)

        #self.trainer = TT(self.select_action, 0.010)
        while self.CarDrive.Run:
            self.select_action()
            time.sleep(0.01)

        #self.trainer.start()

    #실제 실행되는 코드 - 액션을 선택하고 결과를 리턴받음
    def select_action(self):
        #숫자 하나 내보내고, 숫자[9], reward, Done 배열 받음

        e = 0.9 / ((self.play_count / 3) + 1)
        if np.random.rand(1) < e:
            if self.play_count * 3 < self.output_size:
                bound = self.play_count * 3
            else:
                bound = self.output_size
            a = np.random.rand(1) * bound
            action = int(a[0] % self.output_size)
        else:
            action = np.argmax(self.dqn.predict(self.state))

        next_state, self.reward, self.Done = self.step(action)

        self.replay_buffer.append((self.state, action, self.reward, next_state, self.Done))
        self.state = next_state

        if len(self.replay_buffer) > self.R_Memory:
            self.replay_buffer.popleft()

        self.step_count += 1

        if self.step_count % 600 == 0:
            self.replay_train()

        #if self.step_count > 9999999:
           # self.Reset()

        if self.Done:
            self.Reset()

        #if self.play_count > 1000:
        #    self.CarDrive.Stop()

    #C#과 통신
    def step(self, next_move):
        state, reward, Done = self.CarDrive.Move(next_move)
        #if(next_move == 2): print("2")
        #if(next_move == 5): print("5")
        return state, reward, Done

    def Reset(self):
        self.step_count = 0
        self.play_count += 1
        self.replay_train()
        print(self.play_count, " Done\n")
        time.sleep(0.3)
        if not self.CarDrive.Reset(self.play_count):
            self.CarDrive.Stop()

class CConnecter:
    def __init__(self, num):
        self.form = car.Program.ExMain()
        self.Cmodule = self.form.getMainProgram()
        self.Run = True
        self.Set(num)


    def Set(self, carno):
        self.Cmodule.SetCar(carno)

    def Move(self, onehot):
        if self.Cmodule == None:
            self.Stop()
            return None, None, None
        if not self.Cmodule.check():
            self.Stop()
            return None, None, None

        ans = self.Cmodule.Request_Move(onehot)

        state = [[]]
        reward = []
        Done = []

        for car_no in range(len(ans)):
            c_state = ans[car_no].get_Item1()
            for j in range(len(c_state)):
                state[0].append(c_state[j])

            c_reward = ans[car_no].get_Item2()
            reward.append(0 + c_reward)
            Done.append(ans.get_Item3())

        return state, reward, Done

    def Reset(self, count):
        self.Cmodule.playcount(count)
        return self.Cmodule.Reset()

    def Stop(self):
        self.Run = False



#form = CarDrive_1.Program.ExMain()
module = Module(1, 6, 9)
module.trainstart()


