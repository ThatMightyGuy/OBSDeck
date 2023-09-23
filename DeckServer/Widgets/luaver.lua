-- This gets Lua version

local _uuid

-- Ran once on init
function start(uuid)
    _uuid = uuid
end

-- This will be triggered by an external event
function on_update()
    return "<h2 class=\"widget\" id=\"".._uuid.."\">".._VERSION .. "</h2>"
end
