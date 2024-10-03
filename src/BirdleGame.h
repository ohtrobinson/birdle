#pragma once

#include <SDL.h>

namespace birdle {

    class BirdleGame {
    private:
        SDL_Window* _window;

    public:
        BirdleGame();
        ~BirdleGame();

        void Run();
        void Close();
    };

    BirdleGame* Game;

}
