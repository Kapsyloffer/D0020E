import cv2
import numpy
import packages.general.Functions as Functions
import packages.network.SocketO as SocketO
import packages.opencv.Drawtools as Drawtools
import packages.opencv.Masks as Masks
import packages.opencv.FrameObjectSingle as FrameO
import packages.opencv.TrackerO as TrackerO

cam = cv2.VideoCapture(0)

while True:
    ret, frame = cam.read()
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    lower1, upper1 = Masks.maskRedTuned3()
    lower2, upper2 = Masks.maskRedTuned4()
    mask1 = cv2.inRange(hsv, lower1, upper1)
    mask2 = cv2.inRange(hsv, lower2, upper2)
    merged = mask1 + mask2

    # mergedgray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    ret2, thresh = cv2.threshold(merged, 127, 255, 0)
    im2, contours, hierarchy = cv2.findContours(thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    cnt = contours[0]
    M = cv2.moments(cnt)
    # area = cv2.contourArea(cnt)
    # perimeter = cv2.arcLength(cnt, True)
    masked = cv2.bitwise_and(frame, frame, mask=merged)
    if int(M['m00']) != 0:
        cx = int(M['m10'] / M['m00'])
        cy = int(M['m01'] / M['m00'])
        x, y, w, h = cx, cy, 40, 40
        cv2.rectangle(masked, (x, y), ((x-w), (y - h)), (255, 0, 0), 3, 1)
    cv2.drawContours(masked, contours, -1, (0, 255, 0), 3)





    #cv2.imshow("frame", frame)
    cv2.imshow("masked", masked)

    # Y, X = numpy.nonzero(merged)
    # Y, X = numpy.where(numpy.all(merged == numpy.array([255, 255, 255]), axis=0))
    # print(Y, X)

    # cv2.imshow("masked", masked)
    # cv2.imshow("mergec", merged)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
# Make black white mask
# Find white
