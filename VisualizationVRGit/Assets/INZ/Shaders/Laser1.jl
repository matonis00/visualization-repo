include("D:/GIT/INZ/VisualizationRepo/VisualizationVRGit/Assets/INZ/Scripts/laserValueBaseScript.jl")

function Laser1(timeValue::Float64)
    laserIndex = 1;
    ionFile = "Ion.json"
    return laserValue(laserIndex,timeValue,ionFile)
end

println(Laser1(1.499e-9))