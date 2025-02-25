// ---------------------------------------------------------------------
// Copyright (c) 2024 Qualcomm Innovation Center, Inc. All rights reserved.
// SPDX-License-Identifier: BSD-3-Clause
// ---------------------------------------------------------------------
#include "PromptHandler.hpp"
#include "ChatApp.hpp"

using namespace AppUtils;


// Llama3 prompt
constexpr const std::string_view c_first_prompt_prefix_part_1 =
    "<|begin_of_text|><|start_header_id|>system<|end_header_id|>\n\nYou are a sophisticated text analyzer with the capability to discern nuanced forms of argumentative fallacies, specifically focusing on \"ad hominem\" attacks. Your analysis must consider the subtleties of language and context to accurately identify instances where the subject is attacked indirectly through character judgments, stereotypes, profanity, or irrelevant personal details. You MUST ONLY identify ad hominem fallacies that are abusive in nature.  You MUST associate an identified ad hominem with the user who said it.  You MUST ignore any ad hominem that is made in the context of a user seeking clarification of another user's input in the form of a question.  Provide a detailed report in XML format, including the following elements: <report> <d>true ONLY if an ad hominem is detected, OTHERWISE false<d> <a>ONLY if an ad hominem is detected, quote verbatim the part of the sentence containing the ad hominem fallacy, OTHERWISE leave empty.</a> <t>ONLY if an ad hominem is detected, provide the type of ad hominem detected, OTHERWISE leave empty.</t> <e>ONLY if an ad hominem is detected, explain in 1 sentence at most why this is an ad hominem, OTHERWISE leave empty.</e> <rw>ONLY if an ad hominem is detected, provide a rewrite of the sentence to remove the ad hominem, maintaining the original intent but without the fallacy, OTHERWISE leave empty.</rw> </sentence> </report>\n ONLY output the first report.<|eot_id|>\n\n";

constexpr const std::string_view c_prompt_prefix = "<|start_header_id|>user<|end_header_id|>\n\n";
constexpr const std::string_view c_end_of_prompt = "<|eot_id|>";

//
//// Llama3 prompt
//constexpr const std::string_view c_first_prompt_prefix_part_1 =
//    "<|begin_of_text|><|start_header_id|>system<|end_header_id|>\n\nYour name is ";
//constexpr const std::string_view c_first_prompt_prefix_part_2 =
//    "and you are a helpful AI assistant. Please keep answers consice and to the point. <|eot_id|>\n\n";
//constexpr const std::string_view c_prompt_prefix = "<|start_header_id|>user<|end_header_id|>\n\n";
//constexpr const std::string_view c_end_of_prompt = "<|eot_id|>";

/*  Llama2 system prompt
constexpr const std::string_view c_first_prompt_prefix_part_1 = "[INST] <<SYS>>\nYour name is ";
constexpr const std::string_view c_first_prompt_prefix_part_2 =
    "and you are a helpful AI assistant. Please keep answers consice and to the point. \n<</SYS>>\n\n";
constexpr const std::string_view c_prompt_prefix = "[INST] ";
constexpr const std::string_view c_end_of_prompt = " [/INST] ";
*/

PromptHandler::PromptHandler()
    : m_is_first_prompt(true)
{
}

std::string PromptHandler::GetPromptWithTag(const std::string& user_prompt)
{
    // Ref: https://www.llama.com/docs/model-cards-and-prompt-formats/meta-llama-2/
    if (m_is_first_prompt)
    {
        m_is_first_prompt = false;
        return std::string(c_first_prompt_prefix_part_1) + App::c_bot_name.data() +
                user_prompt + c_end_of_prompt.data();
    }
    return std::string(c_prompt_prefix) + user_prompt.data() + c_end_of_prompt.data();
}
