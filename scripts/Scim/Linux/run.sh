#!/bin/bash

SCRIPT_FILE_PATH=$0
SCRIPT_DIRECTORY=`dirname $SCRIPT_FILE_PATH`
EXECUTABLE="SimpleIdServer.Scim.Startup"
EXECUTABLE_DIR="$SCRIPT_DIRECTORY/Server"
EXECUTABLE_PATH="$EXECUTABLE_DIR/$EXECUTABLE"

VERSION=""

pushd $EXECUTABLE_DIR > /dev/null

sleep 2
eval "./$EXECUTABLE --urls=https://localhost:5003";
EXITCODE=$?

popd > /dev/null

exit ${EXITCODE}
