import numpy





def Extrapolate(array, dimensiontuple):
    array = numpy.reshape(array, dimensiontuple)
    x1, y1, x2, y2 = array[-2][0], array[-2][1], array[-1][0], array[-1][1]

    # y = ( deltay / deltax ) * x + b
    # y2 = m*x1 + b
    if x2 - x1 != 0:
        m = int(y2 - y1) / int(x2 - x1)
        mx2 = int(m * x2)
        b = int(y2 + ((-1) * mx2))

        # Extrapolate y3
        x3 = int(x2 + ((x2 - x1) * 0.4))
        y3 = int(m * x3 + b)

        # (x1 + x2 ) /2 , ( y1 + y2) / 2 <-- Midpoint

        return numpy.array([x3, y3, array[-1][2], array[-1][3]]).astype(int)
