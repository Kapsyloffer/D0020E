import socket


class netMan:
    def __init__(self, socket, object):
        self.socket = socket
        self.input = object

    def setSocket(self,socket):
        self.socket = socket
    def setInput(self,input):
        self.input = input
    def transmit(self):
        return