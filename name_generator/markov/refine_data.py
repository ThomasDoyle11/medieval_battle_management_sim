import time
from os import path

file_stem = "english_place_names"
# file_stem = "UK_towns_and_cities"
# file_stem = "US_place_names"
# file_stem = "person_names"

# bool to decide whether to include non-alphabetical characters, or ignore any places with such characters
# e.g. spaces, apostrophes, hyphens...
include_non_alpha = True

acceptable_non_alpha = ["'", "-", " ", "&"]

root_dir = r"C:\Users\thoma\Documents\Python_Projects\PlaceNameGen\PlaceNameGenerator"

def is_acceptable_alpha(name) :
    for i in name :
        if not i.isalpha() and i not in acceptable_non_alpha :
            return False
    return True
            
def refine_data(file_stem, include_non_alpha) :
    print("\n******************************")
    print(" STARTING REFINE DATA PROCESS")
    print("******************************\n")

    # Start timer
    start_time = time.time()

    # Load data
    input_path = root_dir + "/raw_data/" + file_stem + "_raw.txt"
    if not path.exists(input_path) :
        print("Refine data input file does not exist, exiting.")
        return False

    data = open(input_path).readlines()
    for i in range(len(data)) :
        data[i] = data[i].rstrip()

    # Sort into alphabetical order
    # Remove duplicates
    # Remove any with non alphabet characters

    # Start with first valid name
    for i in range(len(data)) :
        if (data[i].isalpha() and not include_non_alpha) or (is_acceptable_alpha(data[i]) and include_non_alpha) :
            sorted_data = [data[i].lower()]
            start_point = i + 1
            break

    for i in range(start_point, len(data)) :
        new_place = data[i].lower()
        if (new_place.isalpha() and not include_non_alpha) or (is_acceptable_alpha(new_place) and include_non_alpha) :
            if new_place not in sorted_data :
                for j in range(len(sorted_data)) :
                    if new_place < sorted_data[j] :
                        sorted_data.insert(j, new_place)
                        break
                    elif j == len(sorted_data) - 1 :
                        sorted_data += [new_place]
                        # print(new_place + " goes at the end.")
            else :
                # print(new_place + " is repeated.")
                None
        else :
            # print(new_place + " contains non-alphabetical character(s).")
            None

    save_location = root_dir + '/data/' + file_stem
    if include_non_alpha :
        save_location += "_non_alpha"
    save_location += '.txt'

    new_file = open(save_location,'w+')
    for i in range(len(sorted_data)) :
        new_file.write(sorted_data[i] + "\n")
    new_file.close()

    end_time = time.time()

    print('Time taken: ' + str(end_time - start_time) + ' seconds')

    print("\n******************************")
    print(" FINISHED REFINE DATA PROCESS")
    print("******************************\n")

    return True

if __name__ == "__main__" :
    refine_data(file_stem, include_non_alpha)