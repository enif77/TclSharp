set {bad var name} {Hello}
set v 123
puts "-x-[set {bad var name}]-x-, world!"
puts "-y-[set {bad ${v}ar name}]-y- world!"

set v XXX
puts $v

set vv aaa[set v]bbb
puts $vv

set vvv [set v]aaa
puts $vvv
