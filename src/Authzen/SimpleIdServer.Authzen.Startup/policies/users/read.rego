package users.read

# import data.shared.helpers

default allow := false

allow if {
  some role
  role = "admin"
  role == input.subject.properties.roles
}