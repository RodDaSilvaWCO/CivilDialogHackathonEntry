// ---------------------------------------------------------------------
// Copyright (c) 2024 Qualcomm Innovation Center, Inc. All rights reserved.
// SPDX-License-Identifier: BSD-3-Clause
// ---------------------------------------------------------------------
#include <windows.h>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <string>

#include "ChatApp.hpp"

namespace
{

    constexpr const std::string_view c_option_genie_config = "--genie-config";
    constexpr const std::string_view c_option_base_dir = "--base-dir";
    constexpr const std::string_view c_option_help = "--help";
    constexpr const std::string_view c_option_help_short = "-h";

    void PrintHelp()
    {
        std::cout << "\n:::::::: Chat with " << App::c_bot_name << " options ::::::::\n\n";
        std::cout << c_option_genie_config << " <Local file path>: [Required] Path to local Genie config for model.\n";
        std::cout << c_option_base_dir
                  << " <Local directory path>: [Required] Base directory to set as the working directory.\n";
        std::cout << "\nDuring chat with " << App::c_bot_name << ", please type " << App::c_exit_prompt
                  << " as a prompt to terminate chat.\n ";
    }

    void PrintWelcomeMessage()
    {
        std::cout << "\n:::::::: Welcome to Chat with " << App::c_bot_name << " ::::::::\n ";
        std::cout << App::c_bot_name << " will use provided configuration file for conversation.\n";
        std::cout << "At any time during chat, please type `" << App::c_exit_prompt
                  << "` to terminate the conversation.\n\n";
        std::cout << "Let's begin with an introduction,\n";
        std::cout << "I'm `" << App::c_bot_name << "`! What's your name? ";
    }

} // namespace




int main(int argc, char* argv[])
{
    std::string genie_config_path;
    std::string base_dir;
    std::string config;
    bool invalid_arguments = false;

    // Check if argument file path is accessible
    auto check_arg_path = [&invalid_arguments](const std::string_view& arg_name, const std::string& value)
    {
        if (value.empty() || !std::filesystem::exists(value))
        {
            std::cout << "\nInvalid argument for " << arg_name
                      << ": It must be a valid and accessible local path. Provided: " << value << std::endl;
            invalid_arguments = true;
        }
    };

    // Arg parser
    for (int i = 1; i < argc; ++i)
    {
        if (c_option_genie_config == argv[i])
        {
            if (i + 1 < argc)
            {
                genie_config_path = argv[++i];
                check_arg_path(c_option_genie_config, genie_config_path);
            }
            else
            {
                std::cout << "\nMissing value for " << c_option_genie_config << " option.\n";
                invalid_arguments = true;
            }
        }
        else if (c_option_base_dir == argv[i])
        {
            if (i + 1 < argc)
            {
                base_dir = argv[++i];
                check_arg_path(c_option_base_dir, base_dir);
            }
            else
            {
                std::cout << "\nMissing value for " << c_option_base_dir << " option.\n";
                invalid_arguments = true;
            }
        }
        else if (c_option_help == argv[i] || c_option_help_short == argv[i])
        {
            PrintHelp();
            return 0;
        }
        else
        {
            std::cout << "Unsupported option " << argv[i] << " provided.\n";
            invalid_arguments = true;
        }
    }

    // If invalid arguments or required arguments are missing, print help and exit.
    if (invalid_arguments || genie_config_path.empty() || base_dir.empty())
    {
        PrintHelp();
        return 1;
    }

    try
    {
        // Load genie_config_path into std::string config before changing directory
        std::ifstream config_file(genie_config_path);
        if (!config_file)
        {
            throw std::runtime_error("Failed to open Genie config file: " + genie_config_path);
        }

        config.assign((std::istreambuf_iterator<char>(config_file)), std::istreambuf_iterator<char>());

        std::filesystem::current_path(base_dir);

        std::string user_name;

        App::ChatApp app(config);

        // Get user name to chat with
        /*PrintWelcomeMessage();
        std::getline(std::cin, user_name);*/

        // Interactive chat
        app.ChatWithUser(/*NULL*/);
    }
    catch (const std::exception& e)
    {
        std::cerr << "Error: " << e.what() << "\n";
        return 1;
    }
    catch (...)
    {
        std::cerr << "Unknown error.\n";
        return 1;
    }
    return 0;
}
