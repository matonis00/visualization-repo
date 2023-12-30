import Base.:+
import Base.:-
import Base.:*
import Base.:/
using JSON

Base.:+(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},
 b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x+b.x,y = a.y+b.y);
Base.:-(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},
 b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x-b.x,y = a.y-b.y);
Base.:*(a::Float64,b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a * b.x,y = a * b.y);
Base.:*(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},b::Float64)::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x * b,y = a.y * b);
Base.:/(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}}, b::Float64)::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x / b, y= a.y / b);

function splineInterpolation(p0::NamedTuple{(:x, :y),Tuple{Float64, Float64}},p1::NamedTuple{(:x, :y),Tuple{Float64, Float64}}
    ,p2::NamedTuple{(:x, :y),Tuple{Float64, Float64}},p3::NamedTuple{(:x, :y),Tuple{Float64, Float64}},t::Float64,piTime::Float64)
    alpha = 1.0;
    tension = 0.0;
    p0 = (x = p0.x, y = p0.y*piTime);
    p1 = (x = p1.x, y = p1.y*piTime);
    p2 = (x = p2.x, y = p2.y*piTime);
    p3 = (x = p3.x, y = p3.y*piTime);
    t01 = distance(p0, p1) ^ alpha;
    t12 = distance(p1, p2) ^ alpha;
    t23 = distance(p2, p3) ^ alpha;

    m1 = (1.0 - tension) * (p2 - p1 + t12 * ((p1 - p0) / t01 - (p2 - p0) / (t01 + t12)));
    m2 = (1.0 - tension) * (p2 - p1 + t12 * ((p3 - p2) / t23 - (p3 - p1) / (t12 + t23)));

    a = 2.0 * (p1 - p2) + m1 + m2;
    b = -3.0 * (p1 - p2) - m1 - m1 - m2;
    c = m1;
    d = p1;
    wynik = a * t * t * t + b * t * t + c * t + d;
    wynik = (x = wynik.x, y = wynik.y/piTime)
    return wynik;
end

function distance(punkt1::NamedTuple{(:x, :y),Tuple{Float64, Float64}}, punkt2::NamedTuple{(:x, :y),Tuple{Float64, Float64}})
    sqrt((punkt2.x - punkt1.x)^2 + (punkt2.y - punkt1.y)^2)
end

function normalization(value::Float64, minValue::Float64, maxValue::Float64)
    return float((value - minValue) / (maxValue - minValue));
end

function laserValue( laserIndex::Int, timeValue::Float64, ionfilePath::String)
    vec = Array{NamedTuple{(:x, :y),Tuple{Float64, Float64}}}[]
    file = JSON.parsefile(ionfilePath)
    pitime = Float64.(get(file,"piTime",0.0))
    for i in get(file, "lasers",[]) 
        tmpvec = NamedTuple{(:x, :y),Tuple{Float64, Float64}}[];
        for a in get(i,"points",[]) 
            push!(tmpvec,(x = Float64.(get(a,"x",0.0)), y = Float64.(get(a,"y",0.0))))
        end
        push!(vec,tmpvec);
    end

    if(timeValue == vec[laserIndex][2].x) 
        return vec[laserIndex][2]
    end

    if(timeValue == vec[laserIndex][length(vec[laserIndex])-1].x) 
        return vec[laserIndex][length(vec[laserIndex])-1]
    end
    
    if(timeValue < vec[laserIndex][2].x) 
        println("Out of MIN range value")
        return
    end
    

    if(timeValue >  vec[laserIndex][length(vec[laserIndex])-1].x)
        println("Out of MAX range value")
        return
    end

    index = 1;
    for p in 2:length(vec[laserIndex])-1
        if(timeValue <= vec[laserIndex][p].x)
            index = p;
            break
        end
    end



    time = timeValue;
    time = normalization(time,vec[laserIndex][index-1].x,vec[laserIndex][index].x)
    odp = splineInterpolation(vec[laserIndex][index-2],vec[laserIndex][index-1],vec[laserIndex][index],vec[laserIndex][index+1],time,pitime/2)
    return odp.y
end
#println(laserValue(1,1.499e-9,"Ion.json"))

