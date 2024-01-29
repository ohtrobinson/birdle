import os

if __name__ == "__main__":
    print("birdle word validator")

    file = None

    while True:
        file = input("Enter the file path to the word list: ")
        if os.path.exists(file):
            break

        print("Can't find that file.")

    known_words = []
    
    print("Validating...")
    has_error = False

    with open(file, 'r', encoding='utf-8-sig') as f:
        line_num = 0

        for line in f:
            line_num += 1

            line = line.strip()

            if line == '' or line.startswith('#'):
                continue

            if len(line) != 5:
                print(f"Line {line_num}: '{line}' is not 5 characters.")
                has_error = True

            if line.lower() != line:
                print(f"Line {line_num}: '{line}' must be lowercase.")
                has_error = True

            if line in known_words:
                print(f"Line {line_num}: '{line}' is a duplicate.")
                has_error = True

            known_words.append(line)

    if has_error:
        print("Validation failed. Please fix all issues and run the script again.")
        exit(1)

    choice = input("Do you want to format the list? THIS WILL OVERWRITE. [Y/n] ").lower()

    if choice == "y":
        with open(file, 'w') as f:
            print("Sorting word list...")
            known_words.sort()

            print("Writing words...")

            for word in known_words:
                f.write(word + '\n')
