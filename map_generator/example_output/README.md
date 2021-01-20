# Example Output

## 0, 1 and 2

These folders all contain images of the generated borders at the given level, with 0 being the highest level (that which has no parent) and 2 being the lowest (that which has no children). In these folders, the individual images are generated in the space of the whole map, i.e. with empty space padding to the edge rather than any other regions of the same level.

## alt

This folder contains the similar structure of '0', '1' and '2' folders, but this time each image has all empty space cut out to the edge of the region, reducing the size of each image. Although this causes a loss of information in each image, the 'regions.json' file contains information such as the x and y bounds of the original region within the whole map, thus the image coupled with this data restores the lost information.

## complete_maps

This folder contains only complete maps, but split into regions for each level.

## border_map.png and coord_map.png

These files help to visualise the data used to prevent the maps. The 'coord_map.png' shows all coordinates used in generating the regions, whilst the 'border_map.png' shows the vectors used and the directions in which they were defined.

## other

There is one file that is generated that I have not included here due to it's larger size (~12MB), and that is the 'map_data.json' file, which essentially encodes every 'pixel' of the final output map as the ID of the lowest region at that pixel.

This tool is exceptionally useful, but not very intuitive to use, especially owing to the complexity of defining the coordinate and vector data for the maps and how prone it is to errors. A nicer way to use this tool would be with an intuitive GUI allowing the manual placement of coordinates, the joining of them with vectors and the manual definition of related data such as names, colours and parent-child relationships. This would allow all the explicit data definition to occur 'under the hood' which would prevent errors and make defining maps more intuitive.
