import socket


class SocketO:
    def __init__(self):
        self.ip = None
        self.port = None
        self.socket = None
        self.AF = socket.AF_INET
        self.protocol = None

    def setIP(self, ip):
        self.ip = ip

    def setPort(self, port):
        self.port = port

    def createSocket(self, ):
        self.socket = socket.socket(self.AF, self.protocol)

    def setTCP(self):
        self.protocol = socket.SOCK_STREAM

    def setUDP(self):
        self.protocol = socket.SOCK_DGRAM
