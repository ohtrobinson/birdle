#pragma once

#include "Window.h"

#include <memory>

namespace birdle {

    class BirdleGame {
    private:
        bool _alive;

    public:
        std::unique_ptr<Window> Window;

        void Run();
        void Close();
    };

    extern BirdleGame Game;

}
