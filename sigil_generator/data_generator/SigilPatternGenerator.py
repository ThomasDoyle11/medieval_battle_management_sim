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

class AllSigilPatterns :
	def __init__(self) :
		self.allSigilPatterns = []

	def AddSigilPattern(self, _patternName, _rectangles, _lines, _circles, _allItemOrganisations) :
		newSigilPattern = OrderedDict()
		newSigilPattern["_patternName"] = _patternName
		newSigilPattern["_rectangles"] = _rectangles
		newSigilPattern["_lines"] = _lines
		newSigilPattern["_circles"] = _circles
		newSigilPattern["_allItemOrganisations"] = _allItemOrganisations

		self.allSigilPatterns += [newSigilPattern]

	def toJSON(self) :
		return json.dumps(self, default=lambda o: o.__dict__, indent=4)

	def save(self) :
		sigil_pattern_json = self.toJSON()
		f = open(r"C:\Users\thoma\Documents\Knight_Manager_All\Python Tools" + r"\sigil_patterns.json", "w+")
		f.write(sigil_pattern_json)
		f.close()
		print("Saved!")

class Rectangles :
	def __init__(self) :
		self.allRectangles = []

	def AddRectangle(self, _centre, _width, _height, _rotation) :
		newRectangle = OrderedDict()
		newRectangle["_centre"] = _centre
		newRectangle["_width"] = _width
		newRectangle["_height"] = _height
		newRectangle["_rotation"] = _rotation

		self.allRectangles += [newRectangle]

class Lines :
	def __init__(self) :
		self.allLines = []

	def AddLine(self, _start, _end, _width) :
		newLine = OrderedDict()
		newLine["_start"] = _start
		newLine["_end"] = _end
		newLine["_width"] = _width

		self.allLines += [newLine]

class Circles :
	def __init__(self) :
		self.allCircles = []

	def AddCircle(self, _centre, _diameter) :
		newCircle = OrderedDict()
		newCircle["_centre"] = _centre
		newCircle["_diameter"] = _diameter

		self.allCircles += [newCircle]

class ItemPlacements :
	def __init__(self) :
		self.allItemPlacements = []

	def AddItemPlacement(self, _centre, _size, _useAltColour) :
		newItemPlacement = OrderedDict()
		newItemPlacement["_centre"] = _centre
		newItemPlacement["_size"] = _size
		newItemPlacement["_useAltColour"] = _useAltColour

		self.allItemPlacements += [newItemPlacement]

class ItemOrganisations :
	def __init__(self) :
		self.allItemOrganisations = []

	def AddItemOrganisation(self, _itemPlacements) :
		newItemOrganisation = OrderedDict()
		newItemOrganisation["_itemPlacements"] = _itemPlacements

		self.allItemOrganisations += [newItemOrganisation]

def Rotate(point, angle, origin) :
	c = math.cos(angle * math.pi / 180)
	s = math.sin(angle * math.pi / 180)

	rel_point = [point[0] - origin[0], point[1] - origin[1]]

	x_new = rel_point[0] * c - rel_point[1] * s
	y_new = rel_point[0] * s + rel_point[1] * c

	new_point = [x_new + origin[0], y_new + origin[1]]

	return new_point

def Stretch(point, scale, origin) :
	rel_point = [point[0] - origin[0], point[1] - origin[1]]

	rel_point = [rel_point[0] * scale, rel_point[1] * scale]

	new_point = [rel_point[0] + origin[0], rel_point[1] + origin[1]]

	return new_point

sigil_center = [0.5, 0.5]

def RotateAboutCenter(point, angle) :
	return Rotate(point, angle, sigil_center)

def StretchAboutCenter(point, scale) :
	return Stretch(point, scale, sigil_center)

diag_scale = math.sqrt(2)

def CommonStretch(point) :
	return StretchAboutCenter(point, diag_scale)

# Generate all possible Sigils
sigilPatternsObj = AllSigilPatterns()

# Generate typical Item Placements
placement_empty = ItemPlacements()

placement_center_large = ItemPlacements()
placement_center_large.AddItemPlacement([0.5,0.5], 0.95, False)

placement_center_large_alt = ItemPlacements()
placement_center_large_alt.AddItemPlacement([0.5,0.5], 0.95, True)

placement_leading_corners = ItemPlacements()
placement_leading_corners.AddItemPlacement([0.25,0.75], 0.475, False)
placement_leading_corners.AddItemPlacement([0.75,0.25], 0.475, False)

placement_leading_corners_alt = ItemPlacements()
placement_leading_corners_alt.AddItemPlacement([0.25,0.75], 0.475, True)
placement_leading_corners_alt.AddItemPlacement([0.75,0.25], 0.475, True)

placement_leading_corners_opp = ItemPlacements()
placement_leading_corners_opp.AddItemPlacement([0.25,0.75], 0.475, False)
placement_leading_corners_opp.AddItemPlacement([0.75,0.25], 0.475, True)

if __name__ == "__main__" :
	# Generate striped Sigils - equally distanced stripes
	for i in range(4) :
		# 4 different rotations
		rotation = 45 * i
		for j in range(2, 6) :
			# Up to 5 stripes (alternating colours)
			stripes = j
			for k in range(2) :
				# Alternate which effect is the background
				newRectangles = Rectangles()
				num_bars = math.ceil(j/2) - k * (j % 2)
				for m in range(num_bars) :
					# Add the rectangles to the Sigil
					base_center = [0.5/j + (2*m+k)/j, 0.5]
					base_width = 1/j
					base_height = 1

					actual_center = RotateAboutCenter(base_center, rotation)
					actual_width = base_width
					actual_height = base_height

					if i % 2 == 1 :
						actual_width = actual_width * diag_scale
						actual_height = actual_height * diag_scale
						actual_center = CommonStretch(actual_center)

					newRectangles.AddRectangle(actual_center, actual_width, actual_height, rotation)
				newItemOrg = ItemOrganisations()
				newItemOrg.AddItemOrganisation(placement_empty.allItemPlacements)
				newItemOrg.AddItemOrganisation(placement_center_large_alt.allItemPlacements)

				if j == 2 :
					# Item Organisations specific to 2 stripes
					base_centers = [[0.25,0.5], [0.75,0.5]]
					actual_centers = [RotateAboutCenter(base_centers[0], rotation), RotateAboutCenter(base_centers[1], rotation)]
					if i % 2 == 1 :
						actual_centers = [CommonStretch(actual_centers[0]), CommonStretch(actual_centers[1])]
					new_placement = ItemPlacements()
					new_placement.AddItemPlacement(actual_centers[0], 0.475, True)
					new_placement.AddItemPlacement(actual_centers[1], 0.475, True)
					newItemOrg.AddItemOrganisation(new_placement.allItemPlacements)

				name = str(j) + " "
				if i == 0 :
					name += "vertical "
				elif i == 2 :
					name += "horizontal "
				elif i % 2 == 1 :
					name += "diagonal "
				name += "stripes"
				sigilPatternsObj.AddSigilPattern(name, newRectangles.allRectangles, [], [], newItemOrg.allItemOrganisations)

	# Generate lined Sigils - like stripes but with thin lines

	# Generate chequered Sigils

	# Generate circled Sigils

sigilPatternsObj.save()
