def trackerstorage():

    return




def boxCordFormatSeb(array):
    array[0] = abs(array[0]-500)
    array[1] = abs((array[1]/2)-250)
    return array

def boxCordsToString(array,format):
    newStr = ""
    if format == "Seb":
        array = boxCordFormatSeb(array)

    for x in array:
        newStr = newStr + str((int(x))) + ":"
   # print(newStr)
    return newStr

def detectRectangleCollision( box1 , box2):
    #collidebox
    b1x ,b1y ,b1w ,b1h = int(box1[0]), int(box1[1]), int(box1[2]), int(box1[3])
    #trackbox
    b2x ,b2y , b2w ,b2h = int(box2[0]), int(box2[1]), int(box2[2]), int(box2[3])

    #bottom left x , y , top right x , y
    #B1 = [ b1x, b1y + b1h , b1x + b1w , b1y]
    #B2 = [b2x , b2y+b2h , b2x +b2w , b2y]

    b1topleft = [b1x,b1y]
    b1botright = [b1x+b1w ,b1y+b1h]
    b2topleft = [b2x,b2y]
    b2botright = [b2x+b2w,b2y+b2h]

    if (b1topleft[0] == b1botright[0] or b1topleft[1] == b1topleft[1] or b2topleft[0] == b2botright[0] or b2topleft[1] == b2botright[1]):
        # the line cannot have positive overlap
        print("Hi1")
        return False

    # If one rectangle is on left side of other
    if (b1topleft[0] >= b2botright[0] or b2topleft[0] >= b1botright[0]):
        print("Hi2")
        return False

    # If one rectangle is above other
    if (b1botright[1] >= b2topleft[1] or b2botright[1] >= b1topleft[1]):
        print("Hi3")
        return False
    return True
