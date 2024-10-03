#include "BirdleGame.h"

#include <stdexcept>
#include <string>

namespace birdle {
    BirdleGame::BirdleGame() {
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0) {
            throw std::runtime_error("Failed to initialize SDL: " + std::string(SDL_GetError()));
        }

        _window =
    }
}
