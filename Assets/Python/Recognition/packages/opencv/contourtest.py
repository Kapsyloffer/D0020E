import numpy
import cv2

blue = numpy.array([255, 0, 0])
green = numpy.array([0, 255, 0])
red = numpy.array([0, 0, 255])

while True:
    frame = cv2.imread('C:\img\center.png')

    imgray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    ret, thresh = cv2.threshold(imgray, 127, 255, 0)
    im2, contours, hierarchy = cv2.findContours(thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)


    cnt = contours[0]
    M = cv2.moments(cnt)
    area = cv2.contourArea(cnt)
    perimeter = cv2.arcLength(cnt, True)
    print(area)



    cv2.drawContours(frame, contours, -1, (0, 255, 0), 3)
    #cv2.drawContours(frame, contours, 3, (0, 255, 0), 3)
    #cv2.drawContours(frame, contours, 2, (0, 255, 0), 3)
    #cv2.drawContours(frame, contours, 1, (0, 255, 0), 3)
    #cnt = contours[4]
    #cv2.drawContours(frame, [cnt], 0, (0, 255, 0), 3)

    cv2.imshow("frame", frame)
    cv2.imshow("im2", im2)


    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
