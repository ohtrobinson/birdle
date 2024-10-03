#pragma once

#include "Math/Size.h"

#include <SDL.h>

#include <string>

namespace birdle {

    struct WindowInfo {
        std::string Title;
        Math::Size Size;
    };

    class Window {
    private:
        SDL_Window* _window;
        SDL_GLContext _glContext;

    public:
        explicit Window(const WindowInfo& info);
        ~Window();

        SDL_Window* Handle();
    };

}

