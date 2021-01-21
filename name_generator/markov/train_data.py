import time
from collections import OrderedDict
import json
from os import path
import refine_data

# Load data
file_stem = "english_place_names"
# file_stem = "UK_towns_and_cities"
# file_stem = "US_place_names"
# file_stem = "person_names"

include_non_alpha = True

# Create json of Markov data with given memory
def add_to_trained_data(trained_data, location) :
    next_level = trained_data
    for i in range(len(location)) :
        if location[i] not in next_level :
            next_level[location[i]] = {}
        next_level = next_level[location[i]]
    
    if 'count' in next_level :
        next_level['count'] += 1
    else :
        next_level['count'] = 1

def train_data(file_stem, include_non_alpha) :
    print("\n*****************************")
    print(" STARTING TRAIN DATA PROCESS")
    print("*****************************\n")
    # Start timer
    start_time = time.time()

    trained_data = {}

    input_path = refine_data.root_dir + '/data/' + file_stem
    if include_non_alpha :
        input_path += "_non_alpha"
    input_path += '.txt'

    if not path.exists(input_path) :
        print("Train data input file does not exist, attempting to create.")
        if refine_data.refine_data(file_stem, include_non_alpha) :
            print("Successfully created refined data, continuing.")
        else :
            print("Failed to create refined data, exiting.")
            return False

    data = open(input_path).readlines()
    for i in range(len(data)) :
        data[i] = data[i].rstrip()

    # Decide on Markov memory of data
    # Memory of 0 implies just counting occurances of each letter
    # Memory of 1 counts how often a letter appears after each given letter
    # Memory of 2 counts how often a letter appears after each given two letter combination etc.
    memory = 5

    for i in range(len(data)) :
        place_name = data[i]
        # print(place_name)
        # Start at -1 to include 'start' character, and end after the last character to include 'end'
        for j in range(-1, len(place_name) + 1) :
            for k in range(memory + 1) :
                if (k <= len(place_name) + 1) :
                    if k <= j :
                        if j == len(place_name) :
                            # This means we're tracing back from the end of the word, so the 'end' char is used
                            location = [place_name[j - k + l] for l in range(k)] + ['end']
                        else :
                            location = [place_name[j - k + l] for l in range(k + 1)]
                        add_to_trained_data(trained_data, location)
                    elif j == k - 1 :
                        # This means we are tracing back before the first letter of the word, thus the 'start' char is used
                        if j == len(place_name) :
                            # This means we're tracing back from the end of the word, so the 'end' char is used
                            location = ['start'] + [place_name[j - k + l + 1] for l in range(k - 1)] + ['end']
                            print(place_name + " is short!")
                        else :
                            location = ['start'] + [place_name[j - k + l + 1] for l in range(k)]
                        add_to_trained_data(trained_data, location)
                    else :
                        # No letter to trace back to
                        None
                else :
                    # Name is too short to reach max memory
                    None

    save_location = refine_data.root_dir + '/trained_data/' + file_stem
    if include_non_alpha :
        save_location += "_non_alpha"
    save_location += '.json'

    new_file = open(save_location,'w+')
    trained_data_json = json.dumps(trained_data, default=lambda o: o.__dict__, indent=4)
    new_file.write(trained_data_json)
    new_file.close()

    end_time = time.time()

    print('Time taken: ' + str(end_time - start_time) + ' seconds')

    print("\n*****************************")
    print(" FINISHED TRAIN DATA PROCESS")
    print("*****************************\n")

    return True

if __name__ == "__main__" :
    train_data(file_stem, include_non_alpha)