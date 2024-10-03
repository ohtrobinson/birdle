#pragma once

#include <SDL.h>

#include <string>

namespace birdle {

    class Window {
    private:
        SDL_Window* _window;
        void* _glContext;

    public:
        Window(const std::string& title);
        ~Window();
    };

}

