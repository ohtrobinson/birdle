#pragma once

#include "Window.h"

namespace birdle {

    class BirdleGame {
    private:


    public:
        BirdleGame();
        ~BirdleGame();

        void Run();
        void Close();
    };

    BirdleGame* Game;

}
