local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawing = require("utils.drawing")

local glider = {}

glider.name = "LeniencyHelper/Entities/UnCeilingBumpableJellyfish"
glider.depth = -5
glider.placements = {
    {
        name = "Un-CeilingBumpable Jellyfish",
        data = {
            Floating = false,
            SpritePath = "objects/glider",
            DisableGroundFriction = true,
            AudioDirectory = "event:/new_content/game/10_farewell/"
        }
    },
}

function glider.sprite(room, entity)
    local bubble = entity.Floating
    local texture = entity.SpritePath .. "/idle0"

    if entity.Floating then
        local x, y = entity.x or 0, entity.y or 0
        local points = drawing.getSimpleCurve({x - 11, y - 1}, {x + 11, y - 1}, {x - 0, y - 6})
        local lineSprites = drawableLine.fromPoints(points):getDrawableSprite()
        local jellySprite = drawableSprite.fromTexture(texture, entity)
        jellySprite:setOffset(jellySprite.meta.width / 2, jellySprite.meta.height / 2 + 4)

        table.insert(lineSprites, 1, jellySprite)

        return lineSprites

    else
        return drawableSprite.fromTexture(texture, entity)
    end
end

function glider.rectangle(room, entity)
    local texture = entity.SpritePath .. "/idle0"
    local sprite = drawableSprite.fromTexture(texture, entity)

    return sprite:getRectangle()
end

return glider