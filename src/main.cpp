#include <memory>

#include "Window.h"

int main(int argc, char* argv[]) {
    birdle::WindowInfo info {
        "Test",
        { 800, 600 }
    };

    auto window = std::make_unique<birdle::Window>(info);

    return 0;
}
