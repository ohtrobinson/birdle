#include "Window.h"

#include <stdexcept>

namespace birdle {
    Window::Window(const WindowInfo& info) {
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0) {
            throw std::runtime_error("Failed to initialize SDL: " + std::string(SDL_GetError()));
        }

        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

        _window = SDL_CreateWindow(info.Title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, info.Size.Width,
                                   info.Size.Height, SDL_WINDOW_OPENGL | SDL_WINDOW_RESIZABLE);

        if (!_window) {
            throw std::runtime_error("Failed to create window: " + std::string(SDL_GetError()));
        }

        _glContext = SDL_GL_CreateContext(_window);
        SDL_GL_MakeCurrent(_window, _glContext);
    }

    Window::~Window() {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }

    SDL_Window* Window::Handle() {
        return _window;
    }
}
