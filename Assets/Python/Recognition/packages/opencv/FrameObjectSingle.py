import cv2
import numpy
import packages.opencv.Masks as Masks
from packages.opencv import Drawtools


class FrameObjectSingle:
    def __init__(self, tag):
        self.frame = None
        self.ret = None
        self.tag = tag
        self.inputSource = None

        self.maskSource1 = None
        self.maskSource2 = None
        self.isMaskSingle = None
        self.colorscheme = None

        self.cx = None
        self.cy = None
        self.point = None

        self.contours = None
        self.midpoint = None

        self.socket = None
        self.socketInput = None

    def getTag(self):
        return self.tag

    def getFrame(self):
        return self.frame

    def getFrameDimensions(self):
        dimensions = self.frame.shape
        return dimensions[0], dimensions[1], dimensions[2]

    def getMidPoint(self):
        return self.midpoint

    def getInputSource(self):
        return self.inputSource

    def getContours(self):
        return self.contours

    def setTag(self, tag):
        self.tag = tag

    def setInput(self, param, *objects):
        if param == "cam":
            self.inputSource = cv2.VideoCapture(0)
        elif param == "frame":
            self.inputSource = objects[0]
        elif param == "img":
            self.frame = cv2.imread(objects[0])

    def setMaskSource(self, param):
        if param == "redtuned":
            self.maskSource1 = Masks.maskRedTuned3()
            self.maskSource2 = Masks.maskRedTuned4()
            self.isMaskSingle = False
        if param == "green":
            self.maskSource1 = Masks.maskGreen1()
            self.maskSource2 = None
            self.isMaskSingle = True

    def setColorScheme(self, param):
        if param == "bgr2hsv":
            self.colorscheme = cv2.COLOR_BGR2HSV

    def setCountours(self):
        ret2, thresh = cv2.threshold(self.frame, 127, 255, 0)
        im2, self.contours, hierarchy = cv2.findContours(thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

        if len(self.contours) != 0:
            cnt = self.contours[0]
            M = cv2.moments(cnt)
            # area = cv2.contourArea(cnt)
            # perimeter = cv2.arcLength(cnt, True)

            # cv2.drawContours(self.frame, self.contours, -1, (0, 255, 0), 3)

            if int(M['m00']) != 0:
                cx = int(M['m10'] / M['m00'])
                cy = int(M['m01'] / M['m00'])
                x, y, w, h = cx, cy, 80, 100
                self.midpoint = numpy.array([x - w, y - h, w, h])
                # cv2.rectangle(self.frame, (x, y), (x + w, y + h), (51, 255, 255), 3)

    def alterShow(self):
        cv2.imshow(self.tag, self.frame)

    def alterReadCam(self):
        self.ret, self.frame = self.inputSource.read()

    def alterBinary(self):
        if not self.isMaskSingle:
            self.frame = Masks.maskMergedGen(self.inputSource, self.colorscheme, self.maskSource1,
                                             self.maskSource2)

    def alterNotShow(self):
        cv2.destroyWindow(self.tag)

    def alterInv(self):
        self.frame = cv2.flip(self.inputSource, 1)

    def alterBinaryAnd(self, frame, mask):
        # Mask = Binary image , 1 where color is within threshhold , else 0
        # Frame = Frame AND ( FRAME AND MASK ) = Only chosen regions filtered by color.
        self.frame = cv2.bitwise_and(frame, frame, mask=mask)
