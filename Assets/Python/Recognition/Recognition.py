import cv2
import packages.network.SocketO as SocketO
import packages.opencv.FrameObjectSingle as FrameO
import packages.opencv.TrackerO as TrackerO

sock = SocketO.SocketO("192.168.10.166", 5065)
sock.setUDP()
sock.createSocket()

cam = FrameO.FrameObjectSingle("cam")
cam.setInput("cam")

inverted = FrameO.FrameObjectSingle("inv")

binaryred = FrameO.FrameObjectSingle("binaryred")
binaryred.setColorScheme("bgr2hsv")
binaryred.setMaskSource("redtuned")

maskedred = FrameO.FrameObjectSingle("maskedred")

# For tracker... pip install opencv-contrib-python==3.4.11.45, or any version < 4
trackO = TrackerO.TrackerO("csrt")

while True:
    # timer = cv2.getTickCount()
    cam.alterReadCam()
    camFrame = cam.getFrame()
    inverted.setInput("frame", camFrame)
    inverted.alterInv()

    binaryred.setInput("frame", camFrame)
    binaryred.alterBinary()
    binaryred.setCountours()

    maskedred.alterBinaryAnd(camFrame, binaryred.getFrame())

    trackO.setTarget(binaryred)
    trackO.setOutPut(maskedred.getFrame())
    trackO.alterUpdate()
    sock.setTransmitObj(trackO)
    sock.transmit("val")

    # Calculate and display fps
    # fps = cv2.getTickFrequency()/(cv2.getTickCount()- timer)
    # cv2.putText(result,str(fps),(75,50),cv2.FONT_HERSHEY_COMPLEX,0.7,(255,255,255),2)

    # cam.alter("show")
    inverted.alterShow()
    binaryred.alterShow()
    maskedred.alterShow()

    # break video capture
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# After the loop release the cap object
# vid.release()
# cv2.destroyAllWindows()
