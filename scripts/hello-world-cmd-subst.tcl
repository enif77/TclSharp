set {bad var name} {Hello}
set v 123
puts "[set {bad var name}], world!"
puts [set {bad ${v}ar name}] world!

set v XXX
puts $v

set vv aaa[set v]bbb
puts $vv

set vvv [set v]aaa
puts $vvv