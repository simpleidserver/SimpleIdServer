package shared.helpers

has_role(role) if {
    role == input.subject.properties.roles[_]
}