-- This is an example clock widget

local _uuid

-- Ran once on init
function start(uuid)
    _uuid = uuid
end

-- This will be triggered by an external event
function on_update()
    local currentTime = os.date('%Y-%m-%d %H:%M:%S')
    return "<h2 class=\"widget\" id=\"".._uuid.."\">"..tostring(currentTime) .. "</h2>"
end
