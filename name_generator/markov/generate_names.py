import json
import random
from os import path
import train_data
import refine_data

file_stem = "english_place_names"
# file_stem = "UK_towns_and_cities"
# file_stem = "US_place_names"
# file_stem = "person_names"
# file_stem = "TES_place_names"
include_non_alpha = True

count = 100

min_len = 4
max_len = 15

memory = 4

def choose_letter(location, ignore_end, force_end, trained_data) :
    next_level = trained_data
    for i in range(len(location)) :
        next_level = next_level[location[i]]
    has_end = 'end' in next_level
    total_instances = next_level['count']

    # Check if 'end' should be ignored as we are below minimum length
    if ignore_end and has_end:
        total_instances -= next_level['end']['count']
    if total_instances == 0 :
        return 'end'

    # Check if 'end' should be forced as we are at maximum length
    if force_end and has_end :
        return 'end'

    # print(new_place)
    rand = random.randint(1, total_instances)
    running_total = 0
    # print("Total: " + str(total_instances))
    # print("Target: : " + str(rand))
    for key, value in next_level.items() :
        if key != 'count' and (key != 'end' or (key == 'end' and not ignore_end)) :
            running_total += value['count']
            # print("Running total: " + str(running_total))
            if rand <= running_total :
                return key
    print("We shouldn't get here.")

def generate_names(file_stem, include_non_alpha, memory, min_len, max_len, count) :
    print("\n*********************************")
    print(" STARTING GENERATE NAMES PROCESS")
    print("*********************************\n")

    output = []

    input_file = refine_data.root_dir + '/trained_data/' + file_stem
    if include_non_alpha :
        input_file += "_non_alpha"
    input_file += '.json'

    if not path.exists(input_file) :
        print("Input file does not exist, attempting to create.")
        if train_data.train_data(file_stem, include_non_alpha) :
            print("Successfully created trained data, continuing.")
        else :
            print("Failed to create trained data, exiting.")
            return False

    with open(input_file,'r') as f:
        trained_data = json.load(f)

    if min_len > max_len :
        min_len = max_len
        print("Min length set to max length as it was greater.")

    for i in range(count) :
        new_place = ""
        new_letter = ""
        while(True) :
            # Generate the current memory to be used for next letter
            cur_len = len(new_place)
            if cur_len < memory :
                location = ['start'] + [new_place[j] for j in range(cur_len)]
            else :
                location = [new_place[-1 - (memory - 1) + i] for i in range(memory)]

            # Check if we're still within min and max length bounds
            if min_len == -1 :
                ignore_end = False
            else :
                ignore_end = cur_len < min_len
            if max_len == -1 :
                force_end = False
            else :
                force_end = cur_len == max_len

            # Choose a new letter
            new_letter = choose_letter(location, ignore_end, force_end, trained_data)

            # Append the letter to the end
            if new_letter != 'end' :
                if not force_end :
                    if new_letter != None :
                        new_place += new_letter
                    else :
                        print("Memory used in Generator is greater than that of the dataset!")
                        memory = cur_len
                        print("Memory set to " + str(memory))
                else :
                    print("New name has reached maximum length without the option of ending.")
                    break
            else :
                if ignore_end :
                    print("New name only has the option of ending and hasn't reached minimum length.")
                break

        output += [new_place]
        print(new_place)

    save_location = refine_data.root_dir + '/output/' + file_stem + "_gen"
    if include_non_alpha :
        save_location += "_non_alpha"
    save_location += '.txt'

    new_file = open(save_location,'w+')
    for i in range(len(output)) :
        new_file.write(output[i] + "\n")
    new_file.close()
    print("\n*********************************")
    print(" FINISHED GENERATE NAMES PROCESS")
    print("*********************************\n")

generate_names(file_stem, include_non_alpha, memory, min_len, max_len, count)