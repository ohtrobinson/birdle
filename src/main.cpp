#include <iostream>
#include <SDL.h>

int main(int argc, char* argv[]) {
    if (SDL_Init(SDL_INIT_VIDEO) < 0) {
        std::cout << "Error initializing SDL: " << SDL_GetError() << std::endl;
        return -1;
    }

    SDL_Window* window = SDL_CreateWindow("Test", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1280, 720, SDL_WINDOW_OPENGL);
    if (!window) {
        std::cout << "Error creating window: "<< SDL_GetError() << std::endl;
        return -1;
    }

    bool alive = true;
    while (alive) {
        SDL_Event event;
        while (SDL_PollEvent(&event)) {
            switch (event.type) {
                case SDL_WINDOWEVENT: {
                    switch (event.window.event) {
                        case SDL_WINDOWEVENT_CLOSE:
                            alive = false;
                            break;
                    }

                    break;
                }
            }
        }
    }

    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}
