import random
import string

import cv2
import numpy as np
import packages.opencv.Drawtools as Drawtools
import packages.opencv.Masks as Masks
import packages.general.Functions as Functions
import packages.opencv.TrackerO as TrackerO
import packages.network.SocketO as SocketO

# StartX , StartY , Width ( from StartX to ... ) , Height ( from StartY to ...)


# initialize tracker


# For tracker... pip install opencv-contrib-python==3.4.11.45
trackObj = TrackerO.TrackerO()
trackObj.createCSRT()
#trackObj.setTrackBox(np.array([287, 150, 130, 130]))
#trackObj.setTrackBox((287,150,130,130))


#tracker = cv2.TrackerCSRT_create()
#trackBox = np.array([150, 150, 100, 100])

#trackers = [cv2.TrackerCSRT_create()]
#trackInit = False


sock = SocketO.SocketO()
sock.setIP("192.168.10.166")
sock.setPort(5065)
sock.setUDP()
sock.createSocket()


# define a video capture object
vid = cv2.VideoCapture(0)



while True:
    #timer = cv2.getTickCount()
    # Capture the video frame by frame
    ret, frame = vid.read()
    inverted = cv2.flip(frame, 1)
    # The black region in the mask has the value of 0,
    # so when multiplied with original image removes all non-blue regions
    result = cv2.bitwise_and(frame, frame, mask=Masks.maskMergedGen(frame,
                                                                    cv2.COLOR_BGR2HSV,
                                                                    Masks.maskRedTuned1(),
                                                                    Masks.maskRedTuned2()))

    #result = Masks.maskGen(frame,cv2.COLOR_BGR2HSV)

    # Tracker
#    if not trackInit:
#        trackBox = cv2.selectROI(result, False, True)
#        trackOk = trackers[-1].init(result, tuple(trackBox))
#        trackInit = True


    if not trackObj.init:
        trackObj.setTrackBox(cv2.selectROI(result, False, True))
        trackObj.trackOk = trackObj.getTracker().init(result, trackObj.getTrackBox())
        trackObj.init = True

    #trackOk, trackBox = trackers[-1].update(result)
    trackOk, trackBox = trackObj.getTracker().update(result)
    trackObj.setTrackBox(np.asarray(trackBox))
    #trackBox = np.asarray(trackBox)

    if trackOk:
        trackBox = trackObj.getTrackBox()
        Drawtools.drawBox(result, trackBox, (0, 255, 0))
        Drawtools.drawMultipleText(result, ("trackBox",
                                            "X: " + str(int(trackBox[0])),
                                            "Xw: " + str(int(trackBox[0]) + int(trackBox[2])),
                                            "Y: " + str(int(trackBox[1])),
                                            "Yh: " + str(int(trackBox[1]) + int(trackBox[3]))), np.array([100, 380]),
                                   (0, 255, 0))
        sock.socket.sendto((Functions.boxCordsToString(trackBox,"Seb")).encode(), (sock.ip, sock.port))

    else:
        # Tracking failure
        Drawtools.drawMultipleText(result,
                                   ("Tracking Failure", "Place object in center. Then press R"),
                                   np.array([100, 380]),(0, 0, 255))
        if cv2.waitKey(1) & 0xFF == ord('r'):
            trackObj.setTrackBox(cv2.selectROI(result, False, True))
            trackObj.createCSRT()
            trackObj.removeFirstTracker()
            trackObj.trackOk = trackObj.getTracker().init(result, trackObj.getTrackBox())


            #trackBox = cv2.selectROI(result, False, True)
            #trackers.append(cv2.TrackerCSRT_create())
            #trackers.remove(trackers[0])
            #trackOk = trackers[-1].init(result, tuple(trackBox))





    # Calculate and display fps
    # fps = cv2.getTickFrequency()/(cv2.getTickCount()- timer)
    # cv2.putText(result,str(fps),(75,50),cv2.FONT_HERSHEY_COMPLEX,0.7,(255,255,255),2)

    # Display options
    # cv2.imshow('frame', frame)
    cv2.imshow('inverted', inverted)
    # cv2.imshow('mask', mask)
    cv2.imshow('result', result)

    # break video capture
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# After the loop release the cap object
#vid.release()
#cv2.destroyAllWindows()
