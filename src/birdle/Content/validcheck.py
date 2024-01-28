import sys

if __name__ == "__main__":
    file = sys.argv[1]

    known_words = []

    with open(file, 'r', encoding='utf-8-sig') as f:
        line_num = 0

        for line in f:
            line_num += 1

            line = line.strip()

            if line == '' or line.startswith('#'):
                continue

            if len(line) != 5:
                print(f"OOPS. Line {line_num}: '{line}' is not 5 characters.")
                break

            if line.lower() != line:
                print(f"OOPS. Line {line_num}: '{line}' must be lowercase.")
                break

            if line in known_words:
                print(f"OOPS. Line {line_num}: '{line}' is a duplicate.")
                break

            known_words.append(line)
