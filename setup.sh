#!/bin/bash

local_path=`pwd`

echo -e "\n# sumi" >> ~/.bash_profile
echo -e "export PATH=${local_path}:\${PATH}" >> ~/.bash_profile
source ~/.bash_profile

echo "setup completed sumi."