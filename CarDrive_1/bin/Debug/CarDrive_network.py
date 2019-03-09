import tensorflow as tf
import random
from collections import deque
import numpy as np
import clr
#clr.AddReference("CarDrive_1")
import Thread_Timer
from threading import Timer


class DQN:
    def __init__(self, session, input_size, output_size):
        self.session = session
        self.input_size = input_size
        self.output_size = output_size
        self.Variable_initializer = tf.tf.contrib.layers.xavier_initializer()

        self._build_network()

    def _build_network(self):
        self.X = tf.placeholder(tf.float32, [None, self.input_size])
        self.Y = tf.placeholder(tf.float32, [None, self.output_size])
        h_size = 10
        l_rate = 0.001

        w1 = self.build_weight("W1", [self.input_size, h_size], self.X)
        w2 = self.build_weight("W2", [h_size, h_size], w1)
        w3 = self.build_weight("W3", [h_size, h_size], w2)
        w4 = tf.get_variable("W4", [h_size, self.output_size], initializer=self.Variable_initializer)

        self.Predict_Q = tf.matmul(w3, w4)

        self.loss_function = tf.reduce_mean(tf.square(self.Y - self.Predict_Q))
        self.train = tf.train.AdamOptimizer(learning_rate=l_rate).minimize(self.loss_function)

    def build_weight(self, name, shape, input):
        w = tf.get_variable(name, shape, initializer=self.Variable_initializer)
        layer = tf.nn.relu(tf.matmul(input, w))
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

        self.dis = 0.5
        self.R_Memory = 50000
        self.play_count = 0
        self.step_count = 0

        self.trainer = None
        self.dqn = None
        self.CarDrive = None
        self.replay_buffer = deque()

        self.state = [[]]
        self.reward = []
        self.Done = []

        self.env_setting()

    def env_setting(self):

        for i in range(self.numofcar):
            self.alivecar.append(True)

        self.dqn = DQN(tf.Session(), self.input_size, self.output_size)
        tf.global_variables_initializer().run()
        #카 드라이브 객체 생성 후 차 갯수 설정
        return True

    #dqn을 훈련시키는 코드
    def replay_train(self):
        replaytime = 5
        for _ in range(replaytime):
            trainbatch = random.sample(self.replay_buffer, 10)

            x_stack = np.empty(0).reshape(0, self.input_size)
            y_stack = np.empty(0).reshape(0, self.output_size)

            for state, action, reward, next_state, done in trainbatch:
                Q = self.dqn.predict(state)

                if done:
                    Q[0, action] = reward
                else:
                    Q[0, action] = (1 - self.dis) * reward + self.dis * np.max(self.dqn.predict(next_state))

                x_stack = np.vstack([x_stack, state])
                y_stack = np.vstack([y_stack, Q])
            loss, _ = self.dqn.update()
            print("Loss : ", loss)

    #훈련
    def trainstart(self):
        self.state, self.reward, self.Done = self.step(0)
        self.trainer = Thread_Timer(self.select_action, 0.010)
        self.trainer.start()
        while self.trainer.isalive:
            pass

    #실제 실행되는 코드 - 액션을 선택하고 결과를 리턴받음
    def select_action(self):
        #숫자 하나 내보내고, 숫자[9], reward, Done 배열 받음

        e = 1. / ((self.play_count / 5) + 1)
        if np.random.rand(1) < e:
            action = np.random.rand(1) % self.output_size
        else:
            action = np.argmax(self.dqn.predict(self.state))

        next_state, self.reward, self.Done = self.step(action)

        self.replay_buffer.append((self.state, action, self.reward, next_state, self.Done))
        self.state = next_state
        if len(self.replay_buffer) > self.R_Memory:
            self.replay_buffer.popleft()

        self.step_count += 1

        for i in len(self.Done):
            if self.Done[i]:
                self.state.pop(i)
                self.reward.pop(i)
                self.Done.pop(i)

        if not self.state:
            self.trainer.stop()
            self.play_count += 1
            print(self.play_count, " Done")
            self.replay_train()
            print("Update")
            Timer(3, self.trainstart()).start()

    #C#과 통신
    def step(self, next_move):
        state = [[]]
        reward = []
        Done = []
        return state, reward, Done


class CConnecter:
    def __init__(self):
        pass

