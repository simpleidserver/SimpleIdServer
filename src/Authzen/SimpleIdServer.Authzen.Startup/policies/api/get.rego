package api.GET.users.__id

default allow := false

allow if {
  user := input.user
  user == "hello"
}