import Base.:+
import Base.:-
import Base.:*
import Base.:/

Base.:+(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},
 b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x+b.x,y = a.y+b.y);
Base.:-(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},
 b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x-b.x,y = a.y-b.y);
Base.:*(a::Float64,b::NamedTuple{(:x, :y),Tuple{Float64, Float64}})::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a * b.x,y = a * b.y);
Base.:*(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}},b::Float64)::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x * b,y = a.y * b);
Base.:/(a::NamedTuple{(:x, :y),Tuple{Float64, Float64}}, b::Float64)::NamedTuple{(:x, :y),Tuple{Float64, Float64}} = (x = a.x / b, y= a.y / b);

function splineInterpolation(p0::NamedTuple{(:x, :y),Tuple{Float64, Float64}},p1::NamedTuple{(:x, :y),Tuple{Float64, Float64}}
    ,p2::NamedTuple{(:x, :y),Tuple{Float64, Float64}},p3::NamedTuple{(:x, :y),Tuple{Float64, Float64}},t::Real)
    alpha = 1.0;
    tension = 0.0;
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
    return wynik;
end

function distance(punkt1::NamedTuple{(:x, :y),Tuple{Float64, Float64}}, punkt2::NamedTuple{(:x, :y),Tuple{Float64, Float64}})
    sqrt((punkt2.x - punkt1.x)^2 + (punkt2.y - punkt1.y)^2)
end

function normalization(value::Float64, minValue::Float64, maxValue::Float64)
    return float((value - minValue) / (maxValue - minValue));
end


pw1 = (x=1.0,y=1.0);
pw2 = (x=2.0,y=2.0)
punkt1 = (x = 3.0, y = 5.0)
punkt2 = (x = 4.0, y = 8.0)
time = 2.9;
wynik = distance(punkt1, punkt2)
println(wynik)
time = normalization(time,pw2.x,punkt1.x)
println(time)
odp = splineInterpolation(pw1,pw2,punkt1,punkt2,time)
println(odp)