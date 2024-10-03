#pragma once

#include <cstdint>

namespace birdle::Math {

    struct Size {
        int32_t Width;
        int32_t Height;

        explicit Size(int32_t wh) {
            Width = wh;
            Height = wh;
        }

        Size(int32_t width, int32_t height) {
            Width = width;
            Height = height;
        }
    };

}