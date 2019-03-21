import threading


class Thread_Timer:
    TimerList = []

    def __init__(self, method, interval=1):
        self.Action = method
        self.Interval = interval
        self.isalive = False

    def set(self, method, interval=1):
        self.Action = method
        self.Interval = interval

    def start(self):
        self.isalive = True

        def t():
            if self.isalive:
                threading.Timer(self.Interval, t).start()
            self.Action()


        t()

    def stop(self):
        self.isalive = False


class Timer(threading.Timer):
    def __init__(self):
        super.__init__()


