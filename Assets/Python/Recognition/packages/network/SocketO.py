from socket import *


class SocketO:
    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.socket = None
        self.AF = AF_INET
        self.protocol = None
        self.transmitObj = None

    def setIP(self, ip):
        self.ip = ip

    def setPort(self, port):
        self.port = port

    def createSocket(self, ):
        self.socket = socket(self.AF, self.protocol)

    def setTCP(self):
        self.protocol = SOCK_STREAM

    def setUDP(self):
        self.protocol = SOCK_DGRAM

    def getSocket(self):
        return self.socket

    def setTransmitObj(self, obj):
        self.transmitObj = obj

    def transmit(self, param):
        x = self.transmitObj.getOutputVal()
        if param == "val" and type(x) == str:
            self.socket.sendto(x.encode('utf8'), (self.ip, self.port))
