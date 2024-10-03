#include "BirdleGame.h"

#include <SDL.h>

#include <stdexcept>

namespace birdle {
    BirdleGame Game;

    void BirdleGame::Run() {
        WindowInfo info {
            "birdle",
            { 800, 600 }
        };

        Window = std::make_unique<birdle::Window>(info);

        _alive = true;

        while (_alive) {
            SDL_Event event;
            while (SDL_PollEvent(&event)) {
                switch (event.type) {
                    case SDL_WINDOWEVENT: {
                        switch (event.window.event) {
                            case SDL_WINDOWEVENT_CLOSE: {
                                _alive = false;
                                break;
                            }
                        }

                        break;
                    }
                }
            }

            if (SDL_GL_SetSwapInterval(1)) {
                throw std::runtime_error("Failed to set swap interval.");
            }
            SDL_GL_SwapWindow(Window->Handle());
        }
    }

    void BirdleGame::Close() {
        _alive = false;
    }
}
