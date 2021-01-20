import random
import time

print("\n***************************************")
print(" STARTING PLACE NAME GENERATOR PROCESS")
print("***************************************\n")

vowels = ['a', 'e', 'i', 'o', 'u']

init_prob = 0.2
prob = init_prob

data = open("./input/place_name_parts.txt").readlines()

places = []
unused = []
changed = []
repititions = 0
total_repititions = 0

for i in range(len(data)) :
    data[i] = data[i].rstrip('\n')

prefix = data[0].split(",")
start = data[1].split(",")
end = data[2].split(",")
suffix = data[3].split(",")

possible = len(start) * len(end) * (len(prefix) * len(suffix) + len(prefix) + len(suffix))
print("Using " + str(len(prefix)) + " prefixes.")
print("Using " + str(len(start)) + " word starts.")
print("Using " + str(len(end)) + " word ends.")
print("Using " + str(len(suffix)) + " suffixes.")
print("For a total of " + str(possible) + " possible combinations.\n")

start_time = time.time()

while(len(places) < 5000) :
    name = ""
    new_prefix = ""
    new_start = ""
    new_end = ""
    new_suffix = ""
    has_changed = False

    rand = random.random()
    if (rand <= prob) :
        new_prefix = random.choice(prefix)

    new_start = random.choice(start)

    new_end = random.choice(end)
    if new_start[-1] == new_end[0] :
        if new_start[-1] in vowels :
            new_start += '-'
            print("Name change: " + new_start)
            has_changed = True
        elif new_start[-2] == new_start[-1] or new_start[-1] == 'w' :
            new_start = new_start[:-1]
            print("Name change: " + new_start)
            has_changed = True
    elif new_start[-1] == 'e' and new_end[0] == 'i' and new_start[-2] not in vowels :
        new_start = new_start[:-1]
        print("Name change: " + new_start)
        has_changed = True

    rand = random.random()
    if (rand <= prob) :
        new_suffix = random.choice(suffix)
    
    name = new_prefix + new_start + new_end + new_suffix

    if name not in places :
        if (new_start.lower() == new_end or new_end == new_suffix.lower().replace(" ","")) :
            unused += [name]
        else :
            places += [name]
            if has_changed :
                changed += [name]
        repititions = 0
    else :
        total_repititions += 1
        repititions += 1
        prob = 1 - (1 - init_prob) ** repititions

end_time = time.time()

new_file = open('./output/output.txt','w+')
for i in range(len(places)) :
    new_file.write(places[i] + "\n")
new_file.close()

new_file = open('./output/unused.txt','w+')
for i in range(len(unused)) :
    new_file.write(unused[i] + "\n")
new_file.close()

new_file = open('./output/changed.txt','w+')
for i in range(len(changed)) :
    new_file.write(changed[i] + "\n")
new_file.close()

print('\nPlaces: ' + str(len(places)))
print('Unused: ' + str(len(unused)))
print('Repititions: ' + str(total_repititions))
print('Time taken: ' + str(end_time - start_time) + ' seconds')

print("\n***************************************")
print(" FINISHED PLACE NAME GENERATOR PROCESS")
print("***************************************\n")