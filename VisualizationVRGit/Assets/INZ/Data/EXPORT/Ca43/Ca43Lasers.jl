include("D:/GIT/INZ/VisualizationRepo/VisualizationVRGit/scripts/laserValueBaseScript.jl")
ionFile = "D:/GIT/INZ/VisualizationRepo/VisualizationVRGit/Assets/INZ/Data/EXPORT/Ca43/Ion.json"
function Laser1(timeValue::Float64)
    laserIndex = 1
    return laserValue(laserIndex,timeValue,ionFile)
end
function Laser2(timeValue::Float64)
    laserIndex = 2
    return laserValue(laserIndex,timeValue,ionFile)
end
function Laser3(timeValue::Float64)
    laserIndex = 3
    return laserValue(laserIndex,timeValue,ionFile)
end
function Laser4(timeValue::Float64)
    laserIndex = 4
    return laserValue(laserIndex,timeValue,ionFile)
end
function Laser5(timeValue::Float64)
    laserIndex = 5
    return laserValue(laserIndex,timeValue,ionFile)
end
