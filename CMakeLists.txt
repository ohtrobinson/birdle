cmake_minimum_required(VERSION 3.20)
project(birdle)

set(CMAKE_CXX_STANDARD 17)

add_executable(birdle src/main.cpp
        src/BirdleGame.cpp
        src/BirdleGame.h
        src/Window.cpp
        src/Window.h
        src/Math/Size.h
)

# ----- SDL2 -----
add_subdirectory(lib/SDL)

if (TARGET SDL2::SDL2main)
    target_link_libraries(birdle PUBLIC SDL2::SDL2main)
endif()

target_link_libraries(birdle PUBLIC SDL2::SDL2-static)