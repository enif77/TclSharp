set {bad var name} {Hello}
set v 123
puts "[set {bad var name}], world!"
puts [set {bad ${v}ar name}] world!
