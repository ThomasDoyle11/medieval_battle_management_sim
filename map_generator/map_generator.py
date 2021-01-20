import math
import time
import os
from os import path
from numpy import random as rd
import numpy as np
import PIL as pillow
from PIL import Image, ImageDraw, ImageFont
from collections import OrderedDict
import json

class AllRegionsObject :
    def __init__(self) :
        self.allRegions = []

    def AddRegion(self, _code, _regionAboveID, _location, _size, _xBounds, _yBounds, _colourID) :
        newRegion = OrderedDict()
        newRegion["_regionID"] = len(self.allRegions)
        newRegion["_code"] = _code
        newRegion["_name"] = region_names[_code]
        newRegion["_location"] = _location
        newRegion["_size"] = _size
        newRegion["_xBounds"] = _xBounds
        newRegion["_yBounds"] = _yBounds
        newRegion["_regionAboveID"] = _regionAboveID
        newRegion["_regionsBelowIDs"] = []
        newRegion["_colourID"] = _colourID

        self.allRegions += [newRegion]

    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, indent=4)

    def GetRegionByCode(self, code) :
        for reg in self.allRegions :
            if reg["_code"] == code :
                return reg
        return None
regionsObj = AllRegionsObject()

# For a given val, will return a colour that is guaranteed to be different from other values of val for the same max_val
# 0 <= val < max_val
def get_col(val, max_val) :
    if val >= max_val :
        print("val should be less than max_val")
        val = val % max_val
    elif val < 0 :
        print("val should be greater than or equal to 0")
        val = val % max_val
    divs = math.ceil(math.pow(max_val, 1/3))
    total = divs ** 3
    val = math.floor(val * (total - 1) / (max_val - 1))


    r = val % divs
    g = (val // divs) % divs
    b = (val // (divs**2)) % divs

    r = math.floor(r * 255 / (divs - 1))
    g = math.floor(g * 255 / (divs - 1))
    b = math.floor(b * 255 / (divs - 1))
    return (r,g,b,255)

# Returns the geometric centroid of an image based on which pixels are fully transparent and which aren't
# Also returns the number of pixels which aren't fully transparent
# Also returns the x and y values of the min and max pixels which are not fully transparent
def get_centroid(im) :
    (X, Y) = map_dims
    m = np.zeros((X, Y))
    size = 0
    x_max = -1
    y_max = -1
    x_min = map_dims[0]
    y_min = map_dims[1]

    for x in range(X):
        for y in range(Y):
            if im[(x, y)] != (0, 0, 0, 0) :
                m[x, y] = 1
                size += 1
                if x > x_max :
                    x_max = x
                elif x < x_min :
                    x_min = x
                if y > y_max :
                    y_max = y
                elif y < y_min :
                    y_min = y
    m = m / np.sum(np.sum(m))

    dx = np.sum(m, 1)
    dy = np.sum(m, 0)

    cx = np.sum(dx * np.arange(X))
    cy = np.sum(dy * np.arange(Y))

    return (cx, cy), size, (x_min, x_max), (y_min, y_max)

# Return the level of a given region
def get_region_level(region) :
    level = 0
    if region["_regionAboveID"] != -1 :
        level = get_region_level(regionsObj.allRegions[region["_regionAboveID"]]) + 1
    else :
        level = 0
    return level

# Get all borders of a region (calls recursively if the region has children)
def get_all_borders(all_regions, str_region) :
    all_borders = []
    if type(all_regions[str_region][0]) == str :
        for i in range(len(all_regions[str_region])) :
            all_borders += get_all_borders(all_regions[str_region][i], str_region)
    else :
        all_borders += [all_regions[str_region]]
    return all_borders

# Generate an image based on the borders of the region, with the given colour as fill
def gen_borders(all_regions, str_region, col) :
    all_borders = get_all_borders(all_regions, str_region)

    # Draw a map with all fractal borders filled in
    whole_image = Image.new('RGBA', map_dims, color = (0,0,0,0))
    whole_draw = ImageDraw.Draw(whole_image)
    for i in range(len(all_borders)) :
        whole_border = []
        for j in range(len(all_borders[i])) :
            # Find the index of the border
            reverse = 1
            # Check if the segment is in the right order, else reverse it
            if j != 0 :
                if all_border_segments[all_borders[i][j]][0] != whole_border[-1] :
                    reverse = -1
            elif len(all_borders[i]) > 1:
                if all_border_segments[all_borders[i][1]][0] != all_border_segments[all_borders[i][0]][-1] and all_border_segments[all_borders[i][1]][-1] != all_border_segments[all_borders[i][0]][-1] :
                    reverse = -1
            whole_border += fractal_borders[all_borders[i][j]][::reverse]

        whole_draw.polygon(whole_border, fill = col)

    return whole_image

# Generate data for each region
# Use the 3-letter code for the region represented it's locations, or the integer ID if use_ID is True
def gen_map_data(all_regions, use_ID) :
    if use_ID :
        map_data = np.full(map_dims, -1, dtype=int)
    else :
        map_data = np.empty(map_dims, dtype="U3")
    
    bottom_borders = get_all_bottom_level_region_codes(all_regions)
    for k in range(len(bottom_borders)) :
        str_region = bottom_borders[k]
        whole_image = gen_borders(all_regions, str_region, (255,255,255,255))
        whole_image_pixels = whole_image.load()
        for i in range(map_dims[0]) :
            for j in range(map_dims[1]) :
                if whole_image_pixels[(i, j)] != (0, 0, 0, 0):
                    if use_ID :
                        if map_data[i][j] == -1 :
                            theRegion = regionsObj.GetRegionByCode(str_region)
                            map_data[i][j] = theRegion["_regionID"]
                        else :
                            None
                    else :
                        if map_data[i][j] == "" :
                            map_data[i][j] = str_region
                        else :
                            None
    return map_data

# Get a list of all regions which contain only border data, and not other regions, which are subregions of str_region
def get_all_str_regions(all_regions, str_region) :
    all_str_regions = []
    if type(all_regions[str_region][0]) == str :
        for i in range(len(all_regions[str_region])) :
            all_str_regions += get_all_str_regions(all_regions, all_regions[str_region][i])
    else :
        all_str_regions += [str_region]
    return all_str_regions

# Return only the regions which contain border data and not other regions
def get_all_bottom_level_regions(all_regions) :
    all_bottom_level_regions = []
    for key in all_regions :
        if type(all_regions[key][0]) != str :
            all_bottom_level_regions += [all_regions[key]]
    return all_bottom_level_regions

# Return only the region codes which contain border data and not other regions
def get_all_bottom_level_region_codes(all_regions) :
    all_bottom_level_regions = []
    for key in all_regions :
        if type(all_regions[key][0]) != str :
            all_bottom_level_regions += [key]
    return all_bottom_level_regions

partial_images = []
# Generate all map data and map images for the given region, and all regions below it
def gen_all_data(all_regions, str_region, region_above_id, map_data) :
    global partial_images
    col = get_col(len(regionsObj.allRegions), num_regions)
    
    all_str_regions = get_all_str_regions(all_regions, str_region)
    whole_image = Image.new('RGBA', map_dims, color = (0,0,0,0))
    # whole_draw = ImageDraw.Draw(whole_image)
    whole_image_pixels = whole_image.load()
    for i in range(map_dims[0]) :
        for j in range(map_dims[1]) :
            if map_data[i][j] in all_str_regions :
                whole_image_pixels[i,j] = col
    centroid, size, x_bounds, y_bounds = get_centroid(whole_image_pixels)
    
    # Invert y_bounds so that the Origin is at the bottom, not top
    y_bounds_alt = (map_dims[1] - y_bounds[1], map_dims[1] - y_bounds[0])

    regionsObj.AddRegion(str_region, region_above_id, centroid, size, x_bounds, y_bounds_alt, col)
    region_id = regionsObj.allRegions[-1]["_regionID"]
    region_level = get_region_level(regionsObj.allRegions[-1])

    name = str(region_level)
    new_dir = map_dir + '/' + name
    if not path.exists(new_dir) :
        os.makedirs(new_dir)
        partial_images += [Image.new('RGBA', map_dims, color = (0,0,0,0))]
    whole_image.save(new_dir + '/' + str(region_id) + '.png')

    partial_images[region_level].paste(whole_image, (0,0), whole_image)

    new_bounds = (x_bounds[0], y_bounds[0], x_bounds[1], y_bounds[1])
    alt_image = whole_image.crop(new_bounds)
    alt_dir = map_dir + '/alt/' + name
    if not path.exists(alt_dir) :
        os.makedirs(alt_dir)
    alt_image.save(alt_dir + '/' + str(region_id) + '.png')


    if (region_above_id != -1) :
        regionsObj.allRegions[region_above_id]["_regionsBelowIDs"] += [region_id]

    print(str_region + " complete!")

    if type(all_regions[str_region][0]) == str :
        for i in range(len(all_regions[str_region])) :
            gen_all_data(all_regions, all_regions[str_region][i], region_id, map_data)

# Create image of all points
def draw_all_coords(all_border_segments, map_dims) :
    new_image = Image.new('RGBA', map_dims, color = (255,255,255,255))
    draw = ImageDraw.Draw(new_image)
        
    new_font = ImageFont.truetype('arial.ttf',size = 15)

    # Draw a grid
    draw.text([5, 5], '0', fill = (0,0,0,256), font = new_font)
    num_x_lines = math.floor(map_dims[0] / 100)
    num_y_lines = math.floor(map_dims[1] / 100)
    for i in range(num_x_lines) :
        draw.line([(i + 1) * 100, 0, ((i + 1) * 100), map_dims[1]], fill = (100,100,100,256), width = 1)
        draw.text([(i + 1) * 100 + 5, 5], str((i + 1) * 100), fill = (0,0,0,256), font = new_font)

    for i in range(num_y_lines) :
        draw.line([0, (i + 1) * 100, map_dims[0], ((i + 1) * 100)], fill = (100,100,100,256), width = 1)
        draw.text([5, (i + 1) * 100 + 5], str((i + 1) * 100), fill = (0,0,0,256), font = new_font)

    for i in range(len(all_border_segments)) :
        for j in range(len(all_border_segments[i])) :
            coords = all_border_segments[i][j]
            draw.rectangle([coords[0]-3,coords[1]-3,coords[0]+3,coords[1]+3],fill = (0,0,0,256))
            draw.text((coords[0]+10,coords[1]), str(i) + '.' + str(j), fill = (0,0,0,256), font = new_font)

    new_image.save(map_dir + '/coord_map.png')

    print('Point map complete.\n')

# Create image of all borders
def draw_all_borders(all_border_segments, map_dims) :
    new_image = Image.new('RGBA', map_dims, color = (256,256,256,256))
    draw = ImageDraw.Draw(new_image)
        
    new_font = ImageFont.truetype('arial.ttf',size = 15)

    draw.text([5, 5], '0', fill = (0,0,0,256), font = new_font)
    num_x_lines = math.floor(map_dims[0] / 100)
    num_y_lines = math.floor(map_dims[1] / 100)
    for i in range(num_x_lines) :
        draw.line([(i + 1) * 100, 0, ((i + 1) * 100), map_dims[1]], fill = (100,100,100,256), width = 1)
        draw.text([(i + 1) * 100 + 5, 5], str((i + 1) * 100), fill = (0,0,0,256), font = new_font)
        
    for i in range(num_y_lines) :
        draw.line([0, (i + 1) * 100, map_dims[0], ((i + 1) * 100)], fill = (100,100,100,256), width = 1)
        draw.text([5, (i + 1) * 100 + 5], str((i + 1) * 100), fill = (0,0,0,256), font = new_font)

    for i in range(len(all_border_segments)) :
        draw.line(all_border_segments[i],fill = (0,0,0,256), width = 3)

        for j in range(len(all_border_segments[i])) :
            # Draw a dot on each vertex
            dot_radius = 3
            ellipse_coords = [all_border_segments[i][j][0] - dot_radius, all_border_segments[i][j][1] - dot_radius, all_border_segments[i][j][0] + dot_radius, all_border_segments[i][j][1] + dot_radius]
            draw.ellipse(ellipse_coords, fill = (0,0,0,256))

            if j + 1 < len(all_border_segments[i]) :
                # Draw an arrow for line direction
                mid_point = [(a + b) / 2 for a,b in zip(all_border_segments[i][j], all_border_segments[i][j+1])]
                line_dir = [a - b for a,b in zip(all_border_segments[i][j+1], all_border_segments[i][j])]
                line_dir = [a / math.sqrt(sum([x**2 for x in line_dir])) for a in line_dir]
                line_norm = [-line_dir[1], line_dir[0]]
                arrow_length = 10
                arrow_point_1 = [a - arrow_length * b - arrow_length * c for a,b,c in zip(mid_point, line_dir, line_norm)]
                arrow_point_2 = [a - arrow_length * b + arrow_length * c for a,b,c in zip(mid_point, line_dir, line_norm)]
                draw.line([tuple(mid_point), tuple(arrow_point_1)], fill = (0,0,0,256), width = 2)
                draw.line([tuple(mid_point), tuple(arrow_point_2)], fill = (0,0,0,256), width = 2)

    new_image.save(map_dir + '/border_map.png')

    print('Border map complete.\n')

# Create fractal borders
def generate_fractal_borders(all_border_segments, all_border_orientations, max_edge_len) :
    fractal_borders = []
    for i in range(len(all_border_segments)) :
        x = [all_border_segments[i][j][0] for j in range(len(all_border_segments[i]))]
        y = [all_border_segments[i][j][1] for j in range(len(all_border_segments[i]))]

        new_x =  list.copy(x)
        new_y = list.copy(y)

        index = 0
        while(index < len(new_x) - 1) :
            x1 = new_x[index]
            x3 = new_x[index + 1]
            y1 = new_y[index]
            y3 = new_y[index + 1]

            r = math.fabs(((x3 - x1) ** 2 + (y3 - y1) ** 2) ** (1/2))

            if r > max_edge_len :

                if x3 - x1 != 0 :
                    theta = math.atan((y3 - y1) / (x3 - x1))
                    sign = math.copysign(1, x3 - x1)
                else :
                    theta = math.pi/2
                    sign = math.copysign(1, y3 - y1)
                
                if use_rand :
                    rand = rd.random()
                else :
                    rand = 1
                
                if all_border_orientations[i] == 0 :
                    mult = (max_outward + max_inward) * rand - max_inward
                elif all_border_orientations[i] == 1 :
                    mult = -((max_outward + max_inward) * rand - max_inward)
                else :
                    if use_rand :
                        rand = rd.random()
                        mult = (2 * max_internal * rand - max_internal)
                    else :
                        mult = 0

                x2 = (x1 + x3) / 2 - mult * sign * r * (2 ** (-2)) * math.sin(theta)
                y2 = (y1 + y3) / 2 + mult * sign * r * (2 ** (-2)) * math.cos(theta)

                new_x.insert(index + 1, x2)
                new_y.insert(index + 1, y2)
            else :
                index += 1

        new_x += [x3]
        new_y += [y3]

        coords = list(zip(new_x, new_y))
        fractal_borders += [coords]
    print('Fractal borders generated.\n')
    return fractal_borders

# Generate all output
def generate_all_output(all_regions, str_region) :
    map_data = gen_map_data(all_regions, False)

    gen_all_data(all_regions, str_region, -1, map_data)

    os.makedirs(map_dir + "/complete_maps")
    for i in range(len(partial_images)) :
        partial_images[i].save(map_dir + '/complete_maps/whole_level_' + str(i) + '.png')

    region_json = regionsObj.toJSON()
    f = open(map_dir + r"\regions.json", "w+")
    f.write(region_json)
    f.close()

    map_data = gen_map_data(all_regions, True)

    map_data_json = json.dumps(map_data.tolist(), default=lambda o: o.__dict__, indent=4)
    f = open(map_dir + r"\map_data.json", "w+")
    f.write(map_data_json)
    f.close()

    print('All maps complete.')
    
# Given two points A1 and A2 and two others B1 and B2, returns true if the normal (anticlockwise from A2 - A1) intersects the line between B1 and B2
# Output is two booleans, whether the intersection occurs, then whether the normal intersects at an an edge (i.e. tangentally)
def does_normal_intersect(A1, A2, B1, B2) :
    if A1 == A2 :
        # print("A1 and A2 are the same point.")
        return False, False
    elif B1 == B2 :
        # print("B1 and B2 are the same point.")
        return False, False
    else :
        # Midpoint of A1, A2
        Am = [(a2 + a1) / 2 for a1, a2 in zip(A1, A2)]
        # Directional Vector from A1 to A2
        Ad = [a2 - a1 for a1, a2 in zip(A1, A2)]
        # Anticlockwise normal of Ad (normalised)
        An = [-Ad[1], Ad[0]]
        lenAn = math.sqrt(sum([an**2 for an in An]))
        An = [an / lenAn for an in An]
        # q is the multiple of the directional vector (B2 - B1) to get from B1 to the intersection point
        # k is the multiple of the normal vector (An) to get from the midpoint (Am) to the intersection point
        denom = (An[1] * (B2[0] - B1[0]) - An[0] * (B2[1] - B1[1]))
        if denom == 0 :
            # print("Normal is parallel to line between B1 and B2.")
            return False, False
        q = (An[0] * (B1[1] - Am[1]) - An[1] * (B1[0] - Am[0])) / denom
        k = ((B2[0] - B1[0]) * (B1[1] - Am[1]) - (B2[1] - B1[1]) * (B1[0] - Am[0])) / denom
        # 0 < q < 1 shows that the intersection occurs between B1 and B2 and not along an extension of the line
        # k > 0 shows that the intersection occurs for the normal facing this way, not the opposite way
        does_intersect = q >= 0 and q <= 1 and k > 0
        at_edge = (q == 0 or q == 1) and k > 0
        return does_intersect, at_edge 

# Calculate whether the point P lies on (d = 0) or either side of the line (d < 0 or d > 0) AB
def signed_distance_from_line(P, A, B) :
    return (P[0] - A[0]) * (B[1] - A[1]) - (P[1] - A[1]) * (B[0] - A[0])

# Calculate whether each line's normal is outward-facing (0), inward-facing (1) or internal (2)
def calculate_border_orientations(all_regions, all_border_segments) :
    all_border_orientations = []
    all_bottom_level_regions = get_all_bottom_level_regions(all_regions)
    for i in range(len(all_border_segments)) :
        # If border is used more than once, it is internal
        border_use_count = 0
        for j in range(len(all_bottom_level_regions)) :
            if i in all_bottom_level_regions[j] :
                border_use_count += 1
                if border_use_count >= 2 :
                    break
                # region_index used to identify Region in next step if border isn't internal
                region_index = j
        if border_use_count >= 2 :
            all_border_orientations += [2]
            continue
        else :
            # Check how many lines the first line of this border's normal intersects of the whole region
            # if this number is even => outwards facing
            # if this number is odd => inwards facing

            border_segment = all_border_segments[i]
            # Pick a single line from the segment to check, all other lines in the segment will have the same orientation
            line_to_compare = border_segment[0:2]
            border_region = all_bottom_level_regions[region_index]

            # Create list of every line in Region
            all_region_lines = []
            for j in range(len(border_region)) :
                reverse = 1
                # Check if the next border segment needs to be reversed
                if j != 0 :
                    if all_border_segments[border_region[j]][0] != all_region_lines[-1] :
                        reverse = -1
                # OR in j == 0 case, check if first line is in opposite direction to next line
                # Only need to check this if region is made up of more than 1 segment
                elif len(border_region) > 1:
                    if all_border_segments[border_region[1]][0] != all_border_segments[border_region[0]][-1] and all_border_segments[border_region[1]][-1] != all_border_segments[border_region[0]][-1] :
                        reverse = -1
                all_region_lines += all_border_segments[border_region[j]][::reverse]
            
            intersection_count = 0
            edge_clip_lines = []
            for j in range(len(all_region_lines) - 1) :
                does_intersect, at_edge = does_normal_intersect(line_to_compare[0], line_to_compare[1], all_region_lines[j], all_region_lines[j+1])
                if does_intersect and not at_edge :
                    intersection_count += 1
                elif at_edge :
                    edge_clip_lines += [(all_region_lines[j], all_region_lines[j+1])]
            while len(edge_clip_lines) > 0 :
                # Extra logic must be performed to check for edge-clipped lines
                # Assuming lines with shared edges are always sequential
                mid_point = [(a + b) / 2 for a,b in zip(line_to_compare[0], line_to_compare[1])]
                first_line = edge_clip_lines.pop(0)
                second_line = edge_clip_lines.pop(0)
                first_line_dir = signed_distance_from_line(first_line[0], first_line[1], mid_point)
                second_line_dir = signed_distance_from_line(second_line[1], second_line[0], mid_point)
                if math.copysign(1, first_line_dir) != math.copysign(1, second_line_dir) :
                    # This imples the normal goes through a node which has a line either side, which is equivalent to intersecting a line
                    # Rather than simply clipping an edge
                    intersection_count += 1
            # Normals will appear flipped in sketches due to the y direction being flipped
            if intersection_count % 2 == 0 :
                all_border_orientations += [0]
            else :
                all_border_orientations += [1]

    return all_border_orientations

# Define all points and borders for the map 'Quandaria', and it's dimensions
def Quandaria_map() :
    all_border_segments = []
    all_regions = {}
    region_names = {"QUA" : "Quandaria", "NTF" : "Northern Fjords", "GFJ" : "Great Fjordor", "BJG" : "Bjurgen", "JHV" : "Johunnvik", "MJH" : "Mjolheim", "EST" : "Eastern Territories", "GRL" : "Grallistan", "SDQ" : "Sardaq", "ARQ" : "Arquubia", "OZS" : "Ozzistan", "SOP" : "Southern Plains", "FIR" : "Firu", "GRS" : "Grassil", "RAQ" : "Raquadorn", "LIG" : "Liraguay", "WSS" : "Western Steppes", "NYA" : "New Yampseco", "RBJ" : "Rocky Bajan", "FLX" : "Flexus", "DRL" : "Drillinoisiana"}
    map_dims = [1000,1000]

    all_regions["QUA"] = ["NTF", "EST", "SOP", "WSS"]

    all_regions["NTF"] = ["GFJ", "BJG", "JHV", "MJH"]
    all_regions["EST"] = ["GRL", "SDQ", "ARQ", "OZS"]
    all_regions["SOP"] = ["FIR", "GRS", "RAQ", "LIG"]
    all_regions["WSS"] = ["NYA", "RBJ", "FLX", "DRL"]

    # Define as a regular diamond split into 16 equal diamond regions
    for i in range(2) :
        # Split into leading and trailing digagonal segments
        for j in range(5) :
            # Split into each diagonal line
            for k in range(4) :
                # Split each diagonal line into 4 segments
                all_border_segments += [[(500 + (2*i-1) * 125 * (-j + k) , 125 * (j + k)), (500 + (2*i-1) * 125 * (-j + k + 1), 125 * (j + k + 1))]]
    
    # Defined NEWS (North East West South)

    all_regions["GFJ"] = [0, 20, 4, 24]    
    all_regions["BJG"] = [4, 21, 8, 25]    
    all_regions["JHV"] = [1, 24, 5, 28]    
    all_regions["MJH"] = [5, 25, 9, 29]

    all_regions["GRL"] = [8, 22, 12, 26]    
    all_regions["SDQ"] = [12, 23, 16, 27]
    all_regions["ARQ"] = [9, 26, 13, 30]    
    all_regions["OZS"] = [13, 27, 17, 31]
    
    all_regions["NYA"] = [2, 28, 6, 32]    
    all_regions["RBJ"] = [6, 29, 10, 33]
    all_regions["FLX"] = [3, 32, 7, 36]    
    all_regions["DRL"] = [7, 33, 11, 37]
    
    all_regions["FIR"] = [10, 30, 14, 34]    
    all_regions["GRS"] = [14, 31, 18, 35]
    all_regions["RAQ"] = [11, 34, 15, 38]    
    all_regions["LIG"] = [15, 35, 19, 39]

    return all_regions, all_border_segments, map_dims, region_names

# Define all points and borders for the map 'Aramaethia', and it's dimensions
def Aramaethia_map() :
    all_border_segments = []
    all_regions = {}
    region_names = {"ARM" : "Aramaethia", "MNL" : "Maineland", "CTR" : "Centrico", "NES" : "Northeastshire", "UQL" : "Upper Quartile", "LQN" : "Lower Quadrant", "CRS" : "The Crescent", "WXT" : "Waxington", "MBC" : "Moonbeach", "SLN" : "Selene", "FTL" : "Fort Twilight", "LNG" : "Longyle", "WLG" : "West Longyle", "HCD" : "Holycradle", "TDW" : "Thunderwater", "DKB" : "Darkberry", "DRP" : "The Drops", "JKC" : "Jykelcent", "RTR" : "Randytim Rock", "CRM" : "Crombal", "LBT" : "Little Bit"}
    map_dims = [1000,1000]

    # Maineland - generate as regular polygon with 3 * (1 + joints) vertices
    joints = 2 # Number of points between connecting border points
    angle = 2 * math.pi / (3 * (joints + 1))
    for j in range(3) :
        all_border_segments += [[(round(350 + 250 * math.cos((i + (1 + joints) * j) * angle)), round(500 - 250 * math.sin((i + (1 + joints) * j) * angle))) for i in range(2 + joints)]]
        all_border_segments += [[(round(350 + 100 * math.cos((i + (1 + joints) * j) * angle)), round(500 - 100 * math.sin((i + (1 + joints) * j) * angle))) for i in range(2 + joints)]]
        all_border_segments += [[(round(350 + (100 + 150 * i) * math.cos((1 + joints) * j * angle)), round(500 - (100 + 150 * i) * math.sin((1 + joints) * j * angle))) for i in range(2)]]
    all_regions["NES"] = [0,5,1,2]
    all_regions["UQL"] = [3,8,4,5]
    all_regions["LQN"] = [6,2,7,8]
    all_regions["CTR"] = [1,4,7]
    all_regions["MNL"] = ["NES", "UQL", "LQN", "CTR"]

    # The Crescent
    all_border_segments += [[(500,800), (650,700), (700,550), (750,450)],
                  [(750,450), (700,300), (750,300), (875,375), (950,600)],
                  [(950,600), (900,700), (900,775)],
                  [(900,775), (800,850), (650,925), (500,800)],
                  [(500,800), (750,775)],
                  [(750,775), (790,650), (800,500)],
                  [(950,600), (800,500)],
                  [(800,500), (750,450)],
                  [(900,775), (750,775)]]

    all_regions["MBC"] = [9, 16, 14, 13]
    all_regions["WXT"] = [10, 15, 16]
    all_regions["SLN"] = [15, 11, 17, 14]
    all_regions["FTL"] = [17, 12, 13]
    all_regions["CRS"] = ["MBC", "WXT", "SLN", "FTL"]

    # Longyle
    all_border_segments += [[(300,200), (200,230), (100,230), (50,150), (150,50),(300,50)],
                  [(300,50), (450,75), (550,60)],
                  [(550,60), (650,60), (725,100)],
                  [(725,100), (800,70), (920,170), (900,265), (800,220), (725,210)],
                  [(725,210), (600,275)],
                  [(600,275), (450,190), (300,200)],
                  [(300,200), (300,50)],
                  [(550,60), (600,275)],
                  [(725,100), (725,210)]]

    all_regions["WLG"] = [18, 24]
    all_regions["HCD"] = [19, 25, 23, 24]
    all_regions["DKB"] = [20, 26, 22, 25]
    all_regions["TDW"] = [21, 26]
    all_regions["LNG"] = ["WLG", "HCD", "DKB", "TDW"]

    # The Drops
    all_border_segments += [[(50,550), (90,580), (100,650), (175,725), (100,780), (40,770), (30,700), (30,650), (46,600), (50,550)],
                  [(250,780), (320,790), (330,860), (300,940), (250,980), (240,860), (200,800), (180,765), (250,780)],
                  [(410,800), (520,880), (520,960), (350,960), (410,800)],
                  [(round(120 + 65 * math.cos(i * 2 * math.pi / 8)), round(895 - 75 * math.sin(i * 2 * math.pi / 8))) for i in range(9)]]
    # Get the current length to add to the indices after creating the border relative to the crescent
    all_regions["JKC"] = [27]
    all_regions["RTR"] = [28]
    all_regions["CRM"] = [29]
    all_regions["LBT"] = [30]
    all_regions["DRP"] = ["JKC", "RTR", "CRM", "LBT"]

    all_regions["ARM"] = ["MNL", "CRS", "LNG", "DRP"]
    return all_regions, all_border_segments, map_dims, region_names

if __name__ == "__main__" :
    print("\n*************************************")
    print(" STARTING FULL MAP GENERATOR PROCESS")
    print("*************************************\n")

    # How drastic the shape change will be, with 0 being none, 1 being medium and >2 being ridiculous
    max_outward = 1
    max_inward = 1
    max_internal = 1
    use_rand = True

    # Decide on where to write maps to (map_dir)
    root_dir = r"your_map_directory_here"
    if not path.exists(root_dir) :
        os.makedirs(root_dir)

    current_dirs = os.listdir(root_dir)
    i = 1
    while(True) :
        name = 'map_' + str(i)
        if name not in current_dirs :
            map_dir = root_dir + name
            os.makedirs(map_dir)
            break
        i += 1

    all_regions, all_border_segments, map_dims, region_names = Aramaethia_map()

    num_regions = len(region_names)

    # The orientation of the anticlockwise normal (0 = outward, 1 = inward, 2 = internal)
    all_border_orientations = calculate_border_orientations(all_regions, all_border_segments)
    draw_all_coords(all_border_segments, map_dims)
    draw_all_borders(all_border_segments, map_dims)
    fractal_borders = generate_fractal_borders(all_border_segments, all_border_orientations, 1)
    generate_all_output(all_regions, "ARM")

    print("\n*************************************")
    print(" FINISHED FULL MAP GENERATOR PROCESS")
    print("*************************************\n")
