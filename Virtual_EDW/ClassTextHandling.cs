﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Virtual_Data_Warehouse
{
    class TextHandling
    {
        /// <summary>
        ///   Retrieves the handlebars variables from a given text box / text.
        /// </summary>
        /// <param name="inputTextBox"></param>
        /// <param name="inputText"></param>
        /// <returns></returns>
        internal static List<string> HandleBarsVariableList(RichTextBox inputTextBox, string inputText)
        {
            List<String> variableList = new List<string>();

            inputTextBox.Clear();

            // Split the text into lines, so we can parse each line
            Regex splitLineRegex = new Regex("\\n");
            String[] lines = splitLineRegex.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                int counter = 0;

                foreach (string token in tokens)
                {
                    if (token == "{")
                    {
                        counter++;
                    }

                    if (counter > 1 && token != "{" && token != "}")
                    {
                        variableList.Add("{{"+token+"}}");
                    }

                    if (token == "}")
                    {
                        counter = 0; //Reset the counter
                    }
                }
            }

            return variableList;
        }

        internal static void SyntaxHighlightHandlebars(RichTextBox inputTextBox, string inputText)
        {
            inputTextBox.Clear();

            // Split the text into lines, so we can parse each line for syntax highlighting
            Regex splitLineRegex = new Regex("\\n");
            String[] lines = splitLineRegex.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                int syntaxHighlightCounter = 0;

                foreach (string token in tokens)
                {
                    // Make sure the default colour is set
                    inputTextBox.SelectionColor = Color.Black;
                    inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Regular);

                    if (token == "{")
                    {
                        syntaxHighlightCounter++;
                    }

                    // Search pattern
                    // Regex handleBarsSyntaxPattern = new Regex("(?<={{).*?(?=}})");
                    // MatchCollection matches = handleBarsSyntaxPattern.Matches(token);

                    //Check whether the token is a keyword, or between {{ }}  
                    String[] keywords = { "#each", "/each", "#if", "/if", "#unless", "/unless" };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            inputTextBox.SelectionColor = Color.DarkGreen;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        } else if (syntaxHighlightCounter>1 && token !="{" && token!="}")
                        {
                            inputTextBox.SelectionColor = Color.Purple;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                        }
                    }

                    inputTextBox.SelectedText = token;

                    if (token == "}")
                    {
                        syntaxHighlightCounter = 0; //Reset the counter
                    }

                }
                inputTextBox.SelectedText = "\n";
            }

        }

        internal static void SyntaxHighlightSql(RichTextBox inputTextBox, string inputText)
        {
            inputTextBox.Clear();

            // Split the text into lines, so we can parse each line for syntax highlighting
            Regex splitLineRegex = new Regex("\\n");
            String[] lines = splitLineRegex.Split(inputText);

            // Outer loop - running through the lines
            foreach (string line in lines)
            {
                // Breaking the line up in elements (tokens)
                Regex wordRegex = new Regex("([ \\t{}():;])");
                String[] tokens = wordRegex.Split(line);

                int syntaxHighlightCounter = 0;

                foreach (string token in tokens)
                {
                    // Make sure the default colour is set
                    inputTextBox.SelectionColor = Color.Black;
                    inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Regular);

                    if (token == "-")
                    {
                        syntaxHighlightCounter++;
                    }

                    // Search pattern
                    // Regex handleBarsSyntaxPattern = new Regex("(?<={{).*?(?=}})");
                    // MatchCollection matches = handleBarsSyntaxPattern.Matches(token);

                    //Check whether the token is a keyword, or between {{ }}  
                    String[] keyWordSql = {
                        "CREATE",
                        "IF",
                        "EXISTS",
                        "DROP",
                        "VIEW",
                        "GO",
                        "DATEADD",
                        "ROW_NUMBER()",
                        "PARTITION",
                        "BY",
                        "TABLE",
                        "UNION",
                        "TRUNCATE",
                        "USE",
                        "GO",
                        "ON",
                        "SELECT",
                        "FROM",
                        "INSERT",
                        "INTO", "OVER", "PARTITION", "IN", "ORDER", "BY", "GROUP", "AS", "WHERE", "NVARCHAR", "NOT EXISTS", "LEFT", "OUTER", "JOIN"};
                    for (int i = 0; i < keyWordSql.Length; i++)
                    {
                        if (keyWordSql[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            inputTextBox.SelectionColor = Color.Blue;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    String[] keyWordFunction = { "HASHBYTES","ROW_NUMBER()", "COALESCE", "CAST" };
                    for (int i = 0; i < keyWordFunction.Length; i++)
                    {
                        if (keyWordFunction[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            inputTextBox.SelectionColor = Color.Purple;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    String[] keyWordFunctionComment = { "--", "/*", "*/" };
                    for (int i = 0; i < keyWordFunctionComment.Length; i++)
                    {
                        if (keyWordFunctionComment[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword.  
                            inputTextBox.SelectionColor = Color.ForestGreen;
                            //inputTextBox.SelectionFont = new Font(inputTextBox.Font, FontStyle.Bold);
                            break;
                        }
                    }

                    inputTextBox.SelectedText = token;

                    if (token == "}")
                    {
                        syntaxHighlightCounter = 0; //Reset the counter
                    }

                }
                inputTextBox.SelectedText = "\n";
            }

        }
    }
}
