
print("\n*******************************************")
print(" STARTING PLACE NAME PARTS CHECKER PROCESS")
print("*******************************************\n")

data = open("./input/place_name_parts.txt").readlines()

for i in range(len(data)) :
    data[i] = data[i].rstrip('\n')

prefix = data[0].split(",")
start = data[1].split(",")
end = data[2].split(",")
suffix = data[3].split(",")
is_duplicates = False

data = [prefix, start, end, suffix]
pre_string = ["Prefix ", "Start ", "End ", "Suffix "]

for k in range(len(data)) :
    for i in range(len(data[k])) :
        for j in range(len(data[k]) - i - 1) :
            if data[k][i] == data[k][i+j+1] :
                print(pre_string[k] + "'" + data[k][i] + "' is a duplicate.")
                is_duplicates = True

if not is_duplicates :
    print("No duplicates found!\n")

new_input = input("Enter any name parts you would like to check for in the file, separated by a comma.\n\n")
pre_string = ["prefix", "start", "end", "suffix"]
checks_found = False

if len(str(new_input)) > 0 :
    checks = new_input.split(",")
    for i in range(len(checks)) :
        for k in range(len(data)) :
            for j in range(len(data[k])) :
                if checks[i].lower().strip(" ") == data[k][j].lower().strip(" ") :
                    print("Place name part '" + data[k][j] + "' appears in " + pre_string[k] + " data.")
                    checks_found = True
    if not checks_found :
        print ("Place name parts do not appear in the data.")
else :
    print("No checks entered.")

print("\n*******************************************")
print(" FINISHED PLACE NAME PARTS CHECKER PROCESS")
print("*******************************************\n")
