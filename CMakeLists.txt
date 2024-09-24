cmake_minimum_required(VERSION 3.29)
project(birdle)

set(CMAKE_CXX_STANDARD 17)

add_executable(birdle src/main.cpp
        src/Engine/App.cpp
        src/Engine/App.h)
