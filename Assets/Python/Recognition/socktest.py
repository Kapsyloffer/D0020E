# from socket import *
import oldsocket

sock = oldsocket.oldsocket()
sock.setIP("192.168.10.166")
sock.setPort(5065)
sock.setUDP()
sock.createSocket()

# sock = socket(AF_INET, SOCK_DGRAM)

while True:
    # sock.sendto(("Hi").encode('utf8'), ("192.168.10.166", 5065))
    sock.socket.sendto(("Hi").encode(), (sock.ip, sock.port))
