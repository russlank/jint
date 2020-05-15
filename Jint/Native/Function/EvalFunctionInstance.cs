﻿using Esprima;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Environments;
using Jint.Runtime.Interpreter.Statements;
using System.Threading.Tasks;

namespace Jint.Native.Function
{
    public sealed class EvalFunctionInstance : FunctionInstance
    {
        private static readonly ParserOptions ParserOptions = new ParserOptions { AdaptRegexp = true, Tolerant = false };
        private static readonly JsString _functionName = new JsString("eval");

        public EvalFunctionInstance(Engine engine, string[] parameters, LexicalEnvironment scope, bool strict) 
            : base(engine, _functionName, parameters, scope, strict)
        {
            _prototype = Engine.Function.PrototypeObject;
            _length = PropertyDescriptor.AllForbiddenDescriptor.NumberOne;
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            return Call(thisObject, arguments, false);
        }

        public JsValue Call(JsValue thisObject, JsValue[] arguments, bool directCall)
        {
            var arg = arguments.At(0);
            if (arg.Type != Types.String)
            {
                return arg;
            }

            var code = TypeConverter.ToString(arg);

            try
            {
                var parser = new JavaScriptParser(code, ParserOptions);
                var program = parser.ParseScript(StrictModeScope.IsStrictModeCode);
                using (new StrictModeScope(program.Strict))
                {
                    using (new EvalCodeScope())
                    {
                        LexicalEnvironment strictVarEnv = null;

                        try
                        {
                            if (!directCall)
                            {
                                Engine.EnterExecutionContext(Engine.GlobalEnvironment, Engine.GlobalEnvironment, Engine.Global);
                            }

                            var lexicalEnvironment = _engine.ExecutionContext.LexicalEnvironment;
                            if (StrictModeScope.IsStrictModeCode)
                            {
                                strictVarEnv = LexicalEnvironment.NewDeclarativeEnvironment(Engine, lexicalEnvironment);
                                Engine.EnterExecutionContext(strictVarEnv, strictVarEnv, Engine.ExecutionContext.ThisBinding);
                            }

                            var argumentsInstance = Engine.DeclarationBindingInstantiation(
                                DeclarationBindingType.EvalCode,
                                program.HoistingScope,
                                functionInstance: this,
                                arguments);

                            var statement = JintStatement.Build(_engine, program);
                            var result = statement.Execute();
                            var value = result.GetValueOrDefault();

                            argumentsInstance?.FunctionWasCalled();

                            if (result.Type == CompletionType.Throw)
                            {
                                var ex = new JavaScriptException(value).SetCallstack(_engine, result.Location);
                                throw ex;
                            }
                            else
                            {
                                return value;
                            }
                        }
                        finally
                        {
                            if (strictVarEnv != null)
                            {
                                Engine.LeaveExecutionContext();
                            }

                            if (!directCall)
                            {
                                Engine.LeaveExecutionContext();
                            }
                        }
                    }
                }
            }
            catch (ParserException e)
            {
                if (e.Description == Messages.InvalidLHSInAssignment)
                {
                    ExceptionHelper.ThrowReferenceError(_engine, (string) null);
                }

                return ExceptionHelper.ThrowSyntaxError<JsValue>(_engine);
            }
        }

        public override Task<JsValue> CallAsync(JsValue thisObject, JsValue[] arguments)
        {
            return CallAsync(thisObject, arguments, false);
        }

        public async Task<JsValue> CallAsync(JsValue thisObject, JsValue[] arguments, bool directCall)
        {
            var arg = arguments.At(0);
            if (arg.Type != Types.String)
            {
                return arg;
            }

            var code = TypeConverter.ToString(arg);

            try
            {
                var parser = new JavaScriptParser(code, ParserOptions);
                var program = parser.ParseScript(StrictModeScope.IsStrictModeCode);
                using (new StrictModeScope(program.Strict))
                {
                    using (new EvalCodeScope())
                    {
                        LexicalEnvironment strictVarEnv = null;

                        try
                        {
                            if (!directCall)
                            {
                                Engine.EnterExecutionContext(Engine.GlobalEnvironment, Engine.GlobalEnvironment, Engine.Global);
                            }

                            var lexicalEnvironment = _engine.ExecutionContext.LexicalEnvironment;
                            if (StrictModeScope.IsStrictModeCode)
                            {
                                strictVarEnv = LexicalEnvironment.NewDeclarativeEnvironment(Engine, lexicalEnvironment);
                                Engine.EnterExecutionContext(strictVarEnv, strictVarEnv, Engine.ExecutionContext.ThisBinding);
                            }

                            var argumentsInstance = Engine.DeclarationBindingInstantiation(
                                DeclarationBindingType.EvalCode,
                                program.HoistingScope,
                                functionInstance: this,
                                arguments);

                            var statement = JintStatement.Build(_engine, program);
                            var result = await statement.ExecuteAsync();
                            var value = result.GetValueOrDefault();

                            argumentsInstance?.FunctionWasCalled();

                            if (result.Type == CompletionType.Throw)
                            {
                                var ex = new JavaScriptException(value).SetCallstack(_engine, result.Location);
                                throw ex;
                            }
                            else
                            {
                                return value;
                            }
                        }
                        finally
                        {
                            if (strictVarEnv != null)
                            {
                                Engine.LeaveExecutionContext();
                            }

                            if (!directCall)
                            {
                                Engine.LeaveExecutionContext();
                            }
                        }
                    }
                }
            }
            catch (ParserException e)
            {
                if (e.Description == Messages.InvalidLHSInAssignment)
                {
                    ExceptionHelper.ThrowReferenceError(_engine, (string)null);
                }

                return ExceptionHelper.ThrowSyntaxError<JsValue>(_engine);
            }
        }
    }
}
